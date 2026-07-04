/*
    Adds OTP attempt-lockout tracking to Users, and updates SP_UserOtp_Validate
    to enforce it. Safe to re-run.

    Behaviour:
      - Each wrong OTP increments Users.OtpFailedAttempts.
      - On reaching @MaxAttempts, Users.OtpLockedUntilUtc is set to now + @LockoutMinutes
        and the account cannot verify OTPs until that time passes (or an admin clears it).
      - A successful verification resets both counters.
      - Attempt/lockout state lives on Users (not UserOtp), so requesting a fresh OTP
        does not reset or bypass the lockout.
      - @MaxAttempts / @LockoutMinutes are passed in by the API (sourced from
        appsettings: Otp:MaxVerifyAttempts, Otp:LockoutMinutes) so they're tunable
        without a DB deployment.
*/

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'OtpFailedAttempts')
BEGIN
    ALTER TABLE dbo.Users ADD OtpFailedAttempts INT NOT NULL CONSTRAINT DF_Users_OtpFailedAttempts DEFAULT (0);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'OtpLockedUntilUtc')
BEGIN
    ALTER TABLE dbo.Users ADD OtpLockedUntilUtc DATETIME2 NULL;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_UserOtp_Validate]
(
    @Mobile         VARCHAR(15),
    @OtpCode        VARCHAR(6),
    @MaxAttempts    INT = 3,
    @LockoutMinutes INT = 60
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId BIGINT = 0;
    DECLARE @LockedUntilUtc DATETIME2 = NULL;
    DECLARE @FailedAttempts INT = 0;

    /*
      Step 1: Resolve UserId + current lockout state from Mobile
    */
    SELECT TOP 1
        @UserId = U.UserId,
        @LockedUntilUtc = U.OtpLockedUntilUtc,
        @FailedAttempts = U.OtpFailedAttempts
    FROM [dbo].[Users] U
    WHERE CONVERT(VARCHAR(15), U.MobileNo) = @Mobile
      AND U.IsActive = 1
      AND U.IsDeleted = 0;

    IF (@UserId = 0)
    BEGIN
        SELECT 0 AS UserId, CAST(0 AS BIT) AS IsLocked, CAST(NULL AS DATETIME2) AS LockedUntilUtc, 0 AS AttemptsRemaining;
        RETURN;
    END

    -- A previous lockout window has elapsed: start a fresh window
    IF (@LockedUntilUtc IS NOT NULL AND @LockedUntilUtc <= SYSUTCDATETIME())
    BEGIN
        SET @LockedUntilUtc = NULL;
        SET @FailedAttempts = 0;

        UPDATE [dbo].[Users]
        SET OtpFailedAttempts = 0, OtpLockedUntilUtc = NULL
        WHERE UserId = @UserId;
    END

    -- Still within an active lockout window
    IF (@LockedUntilUtc IS NOT NULL AND @LockedUntilUtc > SYSUTCDATETIME())
    BEGIN
        SELECT 0 AS UserId, CAST(1 AS BIT) AS IsLocked, @LockedUntilUtc AS LockedUntilUtc, 0 AS AttemptsRemaining;
        RETURN;
    END

    /*
      Step 2: Validate OTP for this UserId
    */
    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UserOtp]
        WHERE UserId = @UserId
          AND OtpHash = @OtpCode
          AND IsUsed = 0
          AND ExpiresAt >= SYSUTCDATETIME()
    )
    BEGIN
        UPDATE [dbo].[UserOtp]
        SET
            IsUsed = 1,
            ExpiresAt = SYSUTCDATETIME()
        WHERE UserId = @UserId
          AND OtpHash = @OtpCode
          AND IsUsed = 0;

        -- Success: reset the attempt counter
        UPDATE [dbo].[Users]
        SET OtpFailedAttempts = 0, OtpLockedUntilUtc = NULL
        WHERE UserId = @UserId;

        SELECT @UserId AS UserId, CAST(0 AS BIT) AS IsLocked, CAST(NULL AS DATETIME2) AS LockedUntilUtc, @MaxAttempts AS AttemptsRemaining;
    END
    ELSE
    BEGIN
        SET @FailedAttempts = @FailedAttempts + 1;

        IF (@FailedAttempts >= @MaxAttempts)
        BEGIN
            SET @LockedUntilUtc = DATEADD(MINUTE, @LockoutMinutes, SYSUTCDATETIME());

            UPDATE [dbo].[Users]
            SET OtpFailedAttempts = @FailedAttempts, OtpLockedUntilUtc = @LockedUntilUtc
            WHERE UserId = @UserId;

            SELECT 0 AS UserId, CAST(1 AS BIT) AS IsLocked, @LockedUntilUtc AS LockedUntilUtc, 0 AS AttemptsRemaining;
        END
        ELSE
        BEGIN
            UPDATE [dbo].[Users]
            SET OtpFailedAttempts = @FailedAttempts
            WHERE UserId = @UserId;

            SELECT 0 AS UserId, CAST(0 AS BIT) AS IsLocked, CAST(NULL AS DATETIME2) AS LockedUntilUtc, (@MaxAttempts - @FailedAttempts) AS AttemptsRemaining;
        END
    END
END;
GO
