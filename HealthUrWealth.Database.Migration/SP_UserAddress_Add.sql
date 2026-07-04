USE [staging_huw]
GO
/****** Object:  StoredProcedure [dbo].[SP_UserAddress_Add]    Script Date: 20-05-2026 20:27:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_UserAddress_Add]
(
    @UserId BIGINT,
    @AddressTypeId INT,
    @CountryId INT,
    @StateId INT,
    @StateName VARCHAR(50),
    @City VARCHAR(100),
    @StreetAddress1 VARCHAR(100),
    @StreetAddress2 VARCHAR(100),
    @LandMark VARCHAR(510),
    @PinCode VARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- If StateId is not provided (NULL or 0) but a StateName is available,
    -- try to resolve StateId from the States table by name.
    IF (@StateId IS NULL OR @StateId = 0) AND (LTRIM(RTRIM(ISNULL(@StateName,''))) <> '')
    BEGIN
        -- Try exact normalized match first
        SELECT TOP 1 @StateId = StateId
        FROM dbo.States
        WHERE ISNULL(IsActive, 1) = 1
          AND ISNULL(IsDeleted, 0) = 0
          AND UPPER(LTRIM(RTRIM(StateName))) = UPPER(LTRIM(RTRIM(@StateName)));

        -- If exact match not found, try a LIKE match (conservative)
        IF @StateId IS NULL OR @StateId = 0
        BEGIN
            SELECT TOP 1 @StateId = StateId
            FROM dbo.States
            WHERE ISNULL(IsActive, 1) = 1
              AND ISNULL(IsDeleted, 0) = 0
              AND UPPER(StateName) LIKE UPPER('%' + LTRIM(RTRIM(@StateName)) + '%');
        END

    -- If we still don't have a valid StateId, fail fast so callers know the state couldn't be resolved
    IF (@StateId IS NULL OR @StateId = 0)
    BEGIN
        RAISERROR('State not found',16,1);
        RETURN;
    END
    END

    INSERT INTO dbo.UserAddresses
    (
        UserId,
        AddressTypeId,
        CountryId,
        StateId,
        StateName,
        City,
        StreetAddress1,
        StreetAddress2,
        LandMark,
        PinCode,
        IsActive,
        IsDeleted,
        CreatedOn,
        UpdatedOn
    )
    VALUES
    (
        @UserId,
        @AddressTypeId,
        @CountryId,
        @StateId,
        @StateName,
        @City,
        @StreetAddress1,
        @StreetAddress2,
        @LandMark,
        @PinCode,
        1,
        0,
        SYSUTCDATETIME(),
        SYSUTCDATETIME()
    );

    SELECT SCOPE_IDENTITY() AS AddressId;
END;
