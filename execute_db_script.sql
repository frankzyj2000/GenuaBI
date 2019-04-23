
-- ================================================
-- Get Total Slots 
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('RPT_SL_GetTotalSlots', 'P') IS NOT NULL
BEGIN
	DROP PROC RPT_SL_GetTotalSlots
	PRINT N'DROP PROC RPT_SL_GetTotalSlots'
END

IF OBJECT_ID('GBI_RPT_SL_GetTotalSlots', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_SL_GetTotalSlots
	PRINT N'DROP PROC GBI_RPT_SL_GetTotalSlots'
END

PRINT N'CREATE PROC GBI_RPT_SL_GetTotalSlots'
GO

CREATE PROCEDURE GBI_RPT_SL_GetTotalSlots
    @DateTimeStart datetime, @DateTimeEnd datetime
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.    
    SET NOCOUNT ON;
	SELECT Count(Distinct IDGM) As TotalSlots
	From dbo.SL_GMInstances 
    WHERE  (SL_GMInstances.DateTimeEnd >=  @DateTimeStart  OR SL_GMInstances.BDEnd = '')
        AND (SL_GMInstances.DateTimeStart <=  @DateTimeEnd)  
END
GO

IF OBJECT_ID('GBI_RPT_MK_PLAYER_PROFILE', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_MK_PLAYER_PROFILE
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_PROFILE'
END

PRINT N'CREATE PROC GBI_RPT_MK_PLAYER_PROFILE'
GO
/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_MK_PLAYER_PROFILE]
   AUTOR:        ALEX      
   FECHA :  20/05/2016 
  ------------------------------------------------------------ */
CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_PROFILE]
(
    @IDPlayer varchar(15)
)
AS
BEGIN
    DECLARE @Err int 
	DECLARE @AccountBalance decimal(18, 2)
	DECLARE @AccountCreationDate Datetime
	DECLARE @Handle decimal(18, 2)
	DECLARE @WinLoss decimal(18, 2)

	--Get Account baance for the player
	select @AccountBalance = sum(Balance), @AccountCreationDate = min(CreationTime) from CF_PlayerAccounts where IDPlayer=@IDPlayer and IsClosed=0 and isFrozen=0
	
	--Get Handle and WinLoss for the player
	select @Handle = sum(Handle), @WinLoss = sum(WinLoss) from MK_PlayerSessionHistory where IDPlayer=@IDPlayer

	--Get player's details
	SELECT MK_Players.IDPlayer,
		   MK_PlayerTitles.Description Title,
		   MK_Players.FirstName,
		   MK_Players.LastName,
		   case when (Gender = 0) then 'Male' else 'Female' end  Gender,
		   MK_Players.BirthdayDate,
		   ActiveCards.BLCardNumber SlotNumber,
		   @AccountBalance AccountBalance,
		   @Handle Handle,
		   @WinLoss Win,
		   IIF(ActiveCards.BLCardNumber IS NULL, 'No', 'Yes') Active,
		   MK_PlayerCategories.Description Category,
		   354 NRCredits,
		   2500 LPAvailable,
		   1440 LPConsumed,
		   'Default' Segment_RFM,
		   'Default' Segment_Classic,
		   MK_Players.Email,
		   MK_Players.Telephone PhoneNumber,
		   MK_Players.CellPhone Cellular,
		   PAddress.Address,
		   PAddress.PostalCode,
		   PAddress.City,
		   PAddress.Country,
		   MK_Players.Occupation,
		   MK_PlayerLanguages.Description Language,
		   MK_Players.NickName,
		   MK_PlayerMaritalStatus.Description MeritalStatus,
		   MK_Players.RFC,
		   MK_Players.CURP,
		   @AccountCreationDate AccountCreationDate,
		   MK_PlayerDocumCategories.Description DocType,
		   MK_Players.DocumentNumber DocNumber

		from MK_Players
			 join MK_PlayerTitles on (MK_Players.IDPlayerTitle = MK_PlayerTitles.IDPlayerTitle)
			 join MK_PlayerLanguages on (MK_Players.IDPlayerLanguage = MK_PlayerLanguages.IDPlayerLanguage)
			 join MK_PlayerCategories on (MK_Players.IDPlayerCategory = MK_PlayerCategories.IDPlayerCategory)
			 join MK_PlayerMaritalStatus on (MK_Players.IDPlayerMaritalStatus = MK_PlayerMaritalStatus.IDPlayerMaritalStatus)
			 join MK_PlayerDocumCategories on (MK_Players.IDPlayerDocumCategory = MK_PlayerDocumCategories.IDPlayerDocumCategory)
			 left outer join (
				select MK_PlayerAddress.IDPlayer, MK_PlayerAddress.Address, MK_PlayerAddress.PostalCode,
					   CFG_CountryCities.Description City, CFG_Countries.Description Country
				  from MK_PlayerAddress, CFG_CountryCities, CFG_Countries
				where MK_PlayerAddress.IDAddressCategory=0
				  and MK_PlayerAddress.IDCountryCity = CFG_CountryCities.IDCountryCity
				  and CFG_CountryCities.IDCountry = CFG_Countries.IDCountry) PAddress on (MK_Players.IDPlayer = PAddress.IDPlayer)
			  left outer join (select top 1 BL_PlayerCards.IDPlayer, BL_PlayerCards.BLCardNumber
				  from BL_PlayerCards, SL_GMLastPlayerCardInserted
				where BL_PlayerCards.IDPlayer=@IDPlayer
				  and BL_PlayerCards.Enabled=1
				  and BL_PlayerCards.BLCardNumber = SL_GMLastPlayerCardInserted.BLCardNumber) ActiveCards on (MK_Players.IDPlayer = ActiveCards.IDPlayer)
		where MK_Players.IDPlayer = @IDPlayer;

	Set @Err = @@Error
	RETURN @Err
End

GO

IF OBJECT_ID('GBI_RPT_MK_PLAYER_ADDINFO', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_MK_PLAYER_ADDINFO
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_ADDINFO'
END

GO

IF OBJECT_ID('GBI_RPT_MK_PLAYER_GAME_HISTORY', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_MK_PLAYER_GAME_HISTORY
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_GAME_HISTORY'
END

PRINT N'CREATE PROC GBI_RPT_MK_PLAYER_GAME_HISTORY'
GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_GAME_HISTORY]
(
    @IDPlayer varchar(15)
)
AS
BEGIN
	DECLARE @Err int
	
	Select Top 100
		MK_PlayerSessionHistory.DateTimeStart,
		SL_GMGames.Description Game,
		SL_GMManufacturers.Description Provider,
		SL_GMTypes.Description GameType,
		MK_PlayerSessionHistory.WinLoss WinLoss,
		MK_PlayerSessionHistory.TotalSecondsPlayed Duration,
		MK_PlayerSessionHistory.Handle Handle,
		0 AverageBet,
		SL_GMs.IDGMFriendly SlotNUmber
	From MK_PlayerSessionHistory
		inner join SL_GMInstances on (MK_PlayerSessionHistory.IDGMInstance = SL_GMInstances.IDGMInstance)
		inner join SL_GMGamesByModels on (SL_GMGamesByModels.IDGMModel = SL_GMInstances.IDGMModel)
		inner join SL_GMGames on (SL_GMGames.IDGMGame = SL_GMGamesByModels.IDGMModel)
		inner join SL_GMs on (SL_GMs.IDGM = SL_GMInstances.IDGM)
		inner join SL_GMTypes on (SL_GMTypes.IDGMType = SL_GMs.IDGMType)
		inner join SL_GMManufacturers on (SL_GMManufacturers.IDGMManufacturer = SL_GMs.IDGMManufacturer)
	Where MK_PlayerSessionHistory.IDPlayer = @IDPlayer
		--and MK_PlayerSessionHistory.DateTimeEnd >= @StartDate
		--and MK_PlayerSessionHistory.DateTimeStart <= @EndDate
	Set @Err = @@Error
	RETURN @Err
END

GO

IF OBJECT_ID('GBI_RPT_MK_PLAYER_CARDS', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_MK_PLAYER_CARDS
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_CARDS'
END

PRINT N'CREATE PROC GBI_RPT_MK_PLAYER_CARDS'
GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_CARDS]
(
    @IDPlayer varchar(15)
)
AS
BEGIN
	DECLARE @Err int
	Select BL_PlayerCards.BLCardNumber,
		Case When (BL_PlayerCards.Enabled = 1) then 'Active' else 'Inactive' end Status,
		Case When (BL_PlayerCards.ManuallyGenerated = 1) then 'Manual' else 'Auto' end IssueMathod,
		AccMovement.FirstUse,
		CF_PlayerAccounts.Balance,
		BL_PlayerCards.IDPlayerAccount
	From BL_PlayerCards
		inner join CF_PlayerAccounts on (CF_PlayerAccounts.IDPlayerAccount = BL_PlayerCards.IDPlayerAccount)
		inner join (select CF_PlayerAccounts.IDPlayerAccount, CardNumber, min(DateTime) FirstUse
	From vw_PlayerAccountMovements, CF_PlayerAccounts
	Where vw_PlayerAccountMovements.IDPlayerAccount = CF_PlayerAccounts.IDPlayerAccount
		and CF_PlayerAccounts.IDPlayer = @IDPlayer
	Group by CF_PlayerAccounts.IDPlayerAccount, CardNumber) AccMovement
			  on (BL_PlayerCards.IDPlayerAccount = AccMovement.IDPlayerAccount)
	Where BL_PlayerCards.IDPLayer = @IDPlayer
	Set @Err = @@Error
	RETURN @Err
END

GO

IF OBJECT_ID('GBI_RPT_MK_PLAYER_CASHDESK', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_MK_PLAYER_CASHDESK
	PRINT N'GBI_RPT_MK_PLAYER_CASHDESK'
END

PRINT N'CREATE PROC GBI_RPT_MK_PLAYER_CASHDESK'
GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_CASHDESK]
(
    @IDPlayer varchar(15)
)
AS
BEGIN
	DECLARE @Err int
	Select CF_TransactionDetails.BLCardNumber,
		CF_Transactions.Time,
		CF_Transactions.TicketNumber,
		CF_Cashiers.UserName Cashier,
		DetailCategories.Name,
		CF_TransactionDetails.AmountIN,
		CF_TransactionDetails.AmountOUT
	From CF_TransactionDetails
		inner join CF_Transactions ON (CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID)
		inner join CF_CashierSessions ON (CF_Transactions.SessionID = CF_CashierSessions.SessionID)
		inner join CF_Cashiers ON (CF_Cashiers.CashierID = CF_CashierSessions.CashierID)
		inner join 
		(
			Select CategoryID, Name
			From CF_TransactionDetailCategories 
			Where CategoryID <>2 AND CategoryID <>3 AND IsHandPayment= 0 AND IsJackpot = 0 AND IsPromo = 0 AND AffectsPlayerAccount = 1
		) As DetailCategories
			ON (DetailCategories.CategoryID = CF_TransactionDetails.CategoryID)
	Where CF_Transactions.AmountIN > 0
		  AND CF_TransactionDetails.PlayerID = @IDPlayer
		  AND (CF_Transactions.IsVoid = 0 OR CF_Transactions.IsVoid is null)
	Set @Err = @@Error
	Return @Err
END

GO

IF OBJECT_ID('GBI_RPT_MK_PLAYER_PROMO', 'P') IS NOT NULL
BEGIN
	DROP PROC GBI_RPT_MK_PLAYER_PROMO
	PRINT N'GBI_RPT_MK_PLAYER_PROMO'
END

PRINT N'GBI_RPT_MK_PLAYER_PROMO'
GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_PROMO]
(
    @IDPlayer varchar(15)
)
AS
BEGIN
	DECLARE @Err int
	Select MKG_PromotionalCreditsReasons.Description Promo,
		MKG_PromotionalCredits.Amount,
		MKG_PromotionalCredits.DateTimeAssigned,
		MKG_PromotionalCredits.BLCardNumberPaid
	From MKG_PromotionalCredits
		inner join MKG_PromotionalCreditsReasons on (MKG_PromotionalCredits.IDReason = MKG_PromotionalCreditsReasons.IDReason) 
	Where IDPlayer = @IDPlayer AND MKG_PromotionalCredits.IDUserCreate IS NOT NULL
	Set @Err = @@Error
	RETURN @Err
END

GO

/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_MK_PLAYER_PREFERENCES]
   AUTOR:        ALEX      
   FECHA :  20/05/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_MK_PLAYER_PREFERENCES', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_PREFERENCES'
	DROP PROC GBI_RPT_MK_PLAYER_PREFERENCES
END

PRINT N'CREAT PROC GBI_RPT_MK_PLAYER_PREFERENCES'

GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_PREFERENCES]
(
    @IDPlayer varchar(15),
	@StartDate datetime,
	@EndDate datetime
)
AS
BEGIN
    DECLARE @Err int

	select SL_GMGames.Description Game,
		   SL_GMManufacturers.Description Provider,
		   SL_GMTypes.Description GameType,
		   sum(MK_PlayerSessionHistory.WinLoss) WinLoss,
		   sum(MK_PlayerSessionHistory.TotalSecondsPlayed) Duration,
		   sum(MK_PlayerSessionHistory.Handle) Handle,
		   0 AverageBet,
		   count(1) Visits
		from MK_PlayerSessionHistory
		inner join SL_GMInstances on (MK_PlayerSessionHistory.IDGMInstance = SL_GMInstances.IDGMInstance)
		inner join SL_GMGamesByModels on (SL_GMGamesByModels.IDGMModel = SL_GMInstances.IDGMModel)
		inner join SL_GMGames on (SL_GMGames.IDGMGame = SL_GMGamesByModels.IDGMModel)
		inner join SL_GMs on (SL_GMs.IDGM = SL_GMInstances.IDGM)
		inner join SL_GMTypes on (SL_GMTypes.IDGMType = SL_GMs.IDGMType)
		inner join SL_GMManufacturers on (SL_GMManufacturers.IDGMManufacturer = SL_GMs.IDGMManufacturer)
	where MK_PlayerSessionHistory.IDPlayer = @IDPlayer
	  and MK_PlayerSessionHistory.DateTimeEnd >= @StartDate
	  and MK_PlayerSessionHistory.DateTimeStart <= @EndDate
	group by SL_GMGames.Description, SL_GMManufacturers.Description, SL_GMTypes.Description

	Set @Err = @@Error
	RETURN @Err
END


GO

/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_MK_PLAYER_TRENDS]
   AUTOR:        ALEX      
   FECHA :  20/05/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_MK_PLAYER_TRENDS', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_TRENDS'
	DROP PROC GBI_RPT_MK_PLAYER_TRENDS
END

PRINT N'CREAT PROC GBI_RPT_MK_PLAYER_TRENDS'

GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_TRENDS]
(
    @IDPlayer varchar(15),
	@StartDate datetime,
	@EndDate datetime
)
AS
BEGIN
	DECLARE @Err int
	select LastWeekDay,
		   sum(Handle) Handle,
		   count(distinct BD) Visits
	from (select Handle, BD, dateadd(ww, datediff(ww, 0, BD), 6) LastWeekDay
		from MK_PlayerSessionHistory
	where MK_PlayerSessionHistory.IDPlayer = @IDPlayer
	  and MK_PlayerSessionHistory.DateTimeEnd >= @StartDate
	  and MK_PlayerSessionHistory.DateTimeStart <= @EndDate) T1
	group by LastWeekDay

	Set @Err = @@Error
	RETURN @Err
END

GO

/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_MK_PLAYER_ACTIVITY]
   AUTOR:        ALEX      
   FECHA :  20/05/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_MK_PLAYER_ACTIVITY', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_MK_PLAYER_ACTIVITY'
	DROP PROC GBI_RPT_MK_PLAYER_ACTIVITY
END

PRINT N'CREAT PROC GBI_RPT_MK_PLAYER_ACTIVITY'

GO
CREATE  PROCEDURE [dbo].[GBI_RPT_MK_PLAYER_ACTIVITY]
(
    @IDPlayer varchar(15),
	@StartDate datetime,
	@EndDate datetime
)
AS
BEGIN
    DECLARE @Err int 
	DECLARE @Handle decimal(18, 2)
	DECLARE @NetWin decimal(18, 2)
	DECLARE @BuyIn decimal(18, 2)
	DECLARE @SessionTime int
	DECLARE @PromoGranted decimal(18, 2)
	DECLARE @PromoIn decimal(18, 2)
	DECLARE @PromoOut decimal(18, 2)
	
	SET @NetWin = 0
	--Get players buy-in
	select @BuyIn = SUM(CF_Transactions.AmountIN)
		from CF_TransactionDetails
		INNER JOIN   CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
	where CF_Transactions.[Time] >= @StartDate 
	  and CF_Transactions.[Time] <= @EndDate
	  and  CF_TransactionDetails.CategoryID in (
		select  CategoryID 
			from CF_TransactionDetailCategories 
		where CategoryID <>2 and  CategoryID <>3 and IsHandPayment= 0 and IsJackpot = 0 and IsPromo = 0 and AffectsPlayerAccount = 1)
		and CF_Transactions.AmountIN > 0
		and CF_TransactionDetails.PlayerID = @IDPlayer
		and (CF_Transactions.IsVoid = 0 OR CF_Transactions.IsVoid is null)
	
	--Get Promo granted to the player
	select @PromoGranted = SUM(Amount)
	   from dbo.MKG_PromotionalCredits 
	where IDPlayer = @IDPlayer AND IDUserCreate IS NOT NULL AND DateTimeAssigned >= @StartDate AND DateTimeAssigned <= @EndDate

	--Get PROMO_IN to the player
	select @PromoIn = SUM(Amount)
		from dbo.MKG_PromotionalCredits 
		INNER JOIN  SL_GMInstances ON SL_GMInstances.IDGMInstance = MKG_PromotionalCredits.IDGMInstancePaid 
		INNER JOIN SL_GMs ON SL_GMInstances.IDGM = SL_GMs.IDGM 
		INNER JOIN SL_GMManufacturers ON SL_GMs.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
	where MKG_PromotionalCredits.IDPlayer = @IDPlayer 
	  AND MKG_PromotionalCredits.DateTimePaid >= @StartDate 
	  AND MKG_PromotionalCredits.DateTimePaid <= @EndDate;

	--Get PROMO_OUT to the player
	Select @PromoOut = SUM(Amount)
		From MKG_PromotionalCredits
		INNER JOIN  SL_GMInstances ON SL_GMInstances.IDGMInstance = MKG_PromotionalCredits.IDGMInstanceCreate 
		INNER JOIN SL_GMs ON SL_GMInstances.IDGM = SL_GMs.IDGM 
		INNER JOIN SL_GMManufacturers ON SL_GMs.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
	Where MKG_PromotionalCredits.IDPlayer = @IDPlayer 
	  AND MKG_PromotionalCredits.DateTimePaid >= @StartDate 
	  AND MKG_PromotionalCredits.DateTimePaid <= @EndDate

	--Get player's activity from sessions
	select @Handle = SUM(Handle),
		   @SessionTime = SUM(TotalSecondsPlayed)
	  from MK_PlayerSessionHistory
	where IDPlayer = @IDPlayer and DateTimeStart <= @EndDate and DateTimeEnd >= @StartDate
	group by IDPlayer


	--Result output
	select ISNULL(@NetWin, 0) NetWin, ISNULL(@Handle, 0) Handle, ISNULL(@BuyIn, 0) BuyIn, ISNULL(@SessionTime, 0) SessionTime, 
		   ISNULL(@PromoGranted, 0) PromoGranted, ISNULL((@PromoIn - @PromoOut), 0) PromoConsumed

	Set @Err = @@Error
	RETURN @Err
End

GO

/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_MK_12]
   Description:  Copied from RPT_MK_12
   AUTOR:        FrankZhang      FECHA :  15/03/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_MK_12', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_MK_12'
	DROP PROC GBI_RPT_MK_12
END

PRINT N'CREATE PROC GBI_RPT_MK_12'
GO
CREATE  PROCEDURE [dbo].[GBI_RPT_MK_12]
(
    @DateStart Datetime,
    @DateEnd Datetime,
    @Qty int,
    @Visit int
)
AS
BEGIN
    DECLARE  @BDStart varchar(8)
    DECLARE  @BDEnd varchar (8)
    DECLARE  @Err int 
    Set @BDStart = dbo.Zero(DATEPART(yy, @DateStart),4) + dbo.Zero(DATEPART(mm, @DateStart),2)+ dbo.Zero(DATEPART(dd, @DateStart),2)
    Set @BDEnd = dbo.Zero(DATEPART(yy, @DateEnd),4) + dbo.Zero(DATEPART(mm, @DateEnd),2)+ dbo.Zero(DATEPART(dd, @DateEnd),2)

    SELECT TOP(@Qty) 
        PlayerName = (LastName + ' ' + FirstName),
        BirthdayDate,
        Gender = Case WHEN Gender = 1 THEN 'M' ELSE 'F' END,
        LastVisit = MAX(BD) ,
        TotalVisits = Count(DISTINCT BD),
        WinLoss = CAST(SUM(WinLoss) AS dec(12,2)),
        Handle = SUM(Handle),
        -- SlotsMillas = SUM(MKS.SlotPoints),
        Telofono = MK_Players.Telephone,
        Celular = MK_Players.CellPhone,
        MK_Players.Email
    FROM MK_PlayerSessionHistory  AS MKS 
		INNER JOIN MK_Players on MKS.IDPlayer = MK_Players.IDPlayer
    Where MKS.BD >= @BDStart AND MKS.BD <= @BDEnd AND RequiresValidation = 0
    /*
    AND  MK_Players.IDPlayer IN 
    ( Select TOP(@Qty) 
    MKP.IDPlayer 
    FROM MK_PlayerSessionHistory MKP 
    where MKP.BD >= @BDStart AND MKP.BD <= @BDEnd AND MKP.RequiresValidation = 0
    GROUP BY MKP.IDPlayer 
    Order by  SUM(Handle) desc )
    */
    GROUP BY LastName+' '+FirstName,Gender,BirthdayDate,
		MK_Players.Telephone,
		MK_Players.CellPhone,
		MK_Players.Email
    --MK_Players.Occupation
    Having  count(DISTINCT BD) > @Visit
    Order BY  SUM(WinLoss) 
    --SUM(WinLoss) / SUM(Handle)    

	Set @Err = @@Error
	RETURN @Err
End

GO


/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_MK_30]
   Description:  Copied from RPT_MK_30
   AUTOR:        FrankZhang      FECHA :  15/03/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_MK_30', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_MK_30'
	DROP PROC GBI_RPT_MK_30
END

PRINT N'CREATE PROC GBI_RPT_MK_30'
GO

CREATE  PROCEDURE [dbo].[GBI_RPT_MK_30]
(
    @BD VarChar(8)
)
AS
BEGIN
    Declare @BD_DateTimeStart as Datetime 
    Declare @BD_DateTimeEnd as Datetime 
    Declare @TotalPlayers as bigint
    declare @TotalSlotsOcuped as Bigint 
    declare @TotalSlots as Bigint 

    Set @BD_DateTimeStart = (Select Top 1 CFG_BusinessDays.DateTimeStart From CFG_BusinessDays where CFG_BusinessDays.BD = @BD)
    Set @BD_DateTimeEnd = (Select Top 1 CFG_BusinessDays.DateTimeEnd From CFG_BusinessDays where CFG_BusinessDays.BD = @BD)

    Set @TotalPlayers = 
    (
        Select  Count ( Distinct Mk.IDPlayer) 
		FROM MK_PlayerSessionHistory  as MK
		WHERE(MK.DateTimeStart between @BD_DateTimeStart and @BD_DateTimeEnd)
    )

										
	SET @TotalSlotsOcuped = 
    (
	    SELECT COUNT (DISTINCT SL_GMInstances.IDGM)
	    FROM MK_PlayerSessionHistory 
	    INNER JOIN  SL_GMInstances ON  SL_GMInstances.IDGMInstance = MK_PlayerSessionHistory.IDGMInstance
	    WHERE (MK_PlayerSessionHistory.DateTimeStart >= @BD_DateTimeStart AND  MK_PlayerSessionHistory.DateTimeEnd<= @BD_DateTimeEnd)
	)

    SET @TotalSlots = 
    (
        SELECT Count(Distinct IDGM) from dbo.SL_GMInstances 
	    WHERE  (SL_GMInstances.DateTimeEnd >=  @BD_DateTimeStart  OR SL_GMInstances.BDEnd = '')
		    AND (SL_GMInstances.DateTimeStart <=  @BD_DateTimeEnd)  )

    DECLARE  @Err int 

    SELECT   
	    Hora = DATEPART(hour, MKS.DateTimeStart), 
	    HoraInicio = Min(MKS.DateTimeStart),
	    HoraFin = Max(MKS.DateTimeStart),
	    Players = Count(Distinct IDPlayer) ,
	    Slots = Count(Distinct SL_GMs.IDGM) ,
	    TotalSession = Count(IDPlayer) ,
	    WinLoss = SUM(MKS.WinLoss) * -1,
	    Handle = SUM(MKS.Handle) * 1,
	    TotalPlayers = @TotalPlayers,
	    TotalSlotsOcuped = @TotalSlotsOcuped,
	    DateTimeStart = @BD_DateTimeStart,
	    DateTimeEnd = @BD_DateTimeEnd,
	    TotalSlots = @TotalSlots
    FROM MK_PlayerSessionHistory AS MKS 
		INNER JOIN SL_GMInstances ON MKS.IDGMInstance = SL_GMInstances.IDGMInstance 
		INNER JOIN SL_GMs ON SL_GMInstances.IDGM = SL_GMs.IDGM 
		INNER JOIN SL_GMManufacturers ON SL_GMs.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer 
    WHERE (MKS.DateTimeStart between @BD_DateTimeStart and @BD_DateTimeEnd)
	    AND  MKS.RequiresValidation = 0
    GROUP BY  DATEPART(hour, MKS.DateTimeStart)
    Order By Min(MKS.DateTimeStart)

	Set @Err = @@Error
	RETURN @Err
End

GO

/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_CF_ResumenOperation]
   Description:  Copied from RPT_CF_ResumenOperation
   AUTOR:        FrankZhang      FECHA :  15/03/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_CF_ResumenOperation', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_CF_ResumenOperation'
	DROP PROC GBI_RPT_CF_ResumenOperation
END

PRINT N'CREATE PROC GBI_RPT_CF_ResumenOperation'
GO

CREATE   PROCEDURE [dbo].[GBI_RPT_CF_ResumenOperation]
    @DateTimeStart datetime, @DateTimeEnd datetime
AS
BEGIN

    DECLARE  @AmountIn decimal(18,2) 
    DECLARE  @UsoInstalaciones decimal(18,2) 
    DECLARE  @AmountOut decimal(18,2) 
    DECLARE  @Taxes decimal(18,2) 
    DECLARE  @TotalSesiones decimal (18,2) 
    DECLARE  @OcupacionActual decimal (18,2)
    DECLARE  @OcupacionPeriodo decimal (18,2)
    DECLARE  @Promociones decimal (18,2)
    DECLARE  @AmountInPlayerAccount decimal (18,2)
    DECLARE  @TotalSlots bigint
    DECLARE  @EspecialPromo decimal(18,2) 
    DECLARE  @CanceledPromo decimal (18,2)
    DECLARE  @NamePromo20 Varchar(300) -- nombre a mostrar en el reporte de las promociones otorgadas por el uso de instalaciones 
    DECLARE  @TotalHP decimal(18,2)  
    DECLARE  @TotalJP decimal(18,2) 
    DECLARE  @TotalSlotsOcuped bigint
    DECLARE  @AmountPlayerAccountVariation decimal (18,2)
    DECLARE  @MontoCajasQueNoAfectanCuentaPlayer decimal (18,2)
    DECLARE  @AmountOutWihtTax decimal(18,2) 
    
    DECLARE  @PlayerAccountIN decimal (18,2)
    DECLARE  @PlayerAccountOUT decimal (18,2)
    DECLARE  @HardSoftCountBill decimal (18,2)
    
    --- set @OcupacionActual = (Select Count (IDPlayerSession) From MK_PlayerSessionHistory where IsOpen = 1 and MK_PlayerSessionHistory.DateTimeStart >= @DateTimeStart)
    --- @OcupacionActual en realidad en esta variable se esta cargado los JP y HP , no se cambio el nombre para no romper Cristal Resport

    SET @OcupacionActual = 
    ( 
        SELECT SUM (CF_TransactionDetails.AmountIn)- SUM (CF_TransactionDetails.AmountOut)
        FROM CF_TransactionDetailCategories
            INNER JOIN  CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
            INNER JOIN   CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
        WHERE 
        (
            CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd 
            AND CF_TransactionDetailCategories.IsHandPayment = 1 AND CF_Transactions.isvoid = 0
        )
        OR
        (
            CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd 
            AND CF_TransactionDetailCategories.IsJackpot = 1 AND CF_Transactions.isvoid = 0
        )         
    )

    SET @OcupacionPeriodo = (
        SELECT COUNT(DISTINCT IDPlayer)
        FROM MK_PlayerSessionHistory 
        WHERE (MK_PlayerSessionHistory.DateTimeStart >= @DateTimeStart AND  MK_PlayerSessionHistory.DateTimeEnd<= @DateTimeEnd)
    )

    --set @TotalSlots = (Select Count (IDGMInstance) From dbo.SL_GMInstances where Enabled = 1)
    --set @Ocupacionperiodo = @Ocupacionperiodo * 100/ @TotalSlots
    --set @OcupacionActual = @OcupacionActual * 100 / @TotalSlots
    
    --SET @HardSoftCountBill =  ISNULL((Select SUM(CF_Vault_DailyMovements.[AmountIN]) from CF_Vault_DailyMovements where 
    --CF_Vault_DailyMovements.[TypeID] = 4 and CF_Vault_DailyMovements.[DateTime] >= @DateTimeStart 
    --     AND CF_Vault_DailyMovements.[DateTime] <= @DateTimeEnd),0)
     
    SET @HardSoftCountBill = ISNULL
    (
        (
            SELECT  SUM (CF_HardSoftCountGMs.TotalAmount)    
            FROM CF_HardSoftCount INNER JOIN  CF_HardSoftCountGMs ON CF_HardSoftCount.IDHSCount = CF_HardSoftCountGMs.IDHSCount      
            Where  CF_HardSoftCount.DateTimeStart >= @DateTimeStart AND CF_HardSoftCount.DateTimeStart <= @DateTimeEnd
        ), 0
    )
     
    --@AmountIn Todas las entradas cash
    SET @AmountIn = ISNULL
    (
        (
            SELECT SUM (CF_TransactionDetails.AmountIn)
            FROM CF_TransactionDetailCategories
                INNER JOIN  CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
                INNER JOIN   CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
            WHERE  CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd
                AND  CF_TransactionDetails.CategoryID <>2
                AND CF_TransactionDetailCategories.IsHandPayment= 0 
                AND CF_TransactionDetailCategories.IsJackpot = 0 
                AND CF_TransactionDetailCategories.IsPromo = 0 
                AND CF_Transactions.IsVoid <> 1
        ), 0
     )

    -- Le sumo al total de entradas de cajas lo tranferido a boveda por el conteo de Maquinas    
    SET @AmountIn = @AmountIn + @HardSoftCountBill
    
    SET @EspecialPromo = ISNULL
    (
        (
            SELECT SUM (CF_TransactionDetails.AmountIn)
            FROM CF_TransactionDetailCategories
                INNER JOIN  CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
                INNER JOIN   CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
            WHERE  CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd
				AND  CF_TransactionDetails.CategoryID = 2 AND CF_Transactions.IsVoid <> 1
        ), 0
    )
         
    SET @AmountOut = ISNULL
    (
        (
            SELECT SUM(CF_TransactionDetails.AmountOut)
            FROM CF_TransactionDetailCategories
                INNER JOIN CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
                INNER JOIN CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
            WHERE CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd 
                AND  CF_TransactionDetails.CategoryID <>2
                AND CF_TransactionDetailCategories.IsHandPayment= 0  
                AND CF_TransactionDetailCategories.IsJackpot = 0 
                AND CF_TransactionDetailCategories.IsPromo = 0 
                AND CF_Transactions.IsVoid <> 1
        ), 0
    )

    SET @Taxes = ISNULL
    (
        (
            SELECT SUM(CF_TaxTransactionDetails.Amount) 
            FROM CF_Taxes 
                INNER JOIN CF_TaxTransactionDetails ON CF_TaxTransactionDetails.taxID = CF_Taxes.TaxID 
                INNER JOIN CF_Transactions ON CF_TaxTransactionDetails.TransactionID = CF_Transactions.TransactionID
            WHERE CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd AND CF_Transactions.IsVoid <> 1
        ), 0
    )
    
    SET @AmountOutWihtTax = ISNULL
    (   
        (
            SELECT SUM(CF_Transactions.AmountOut)
            FROM CF_TransactionDetailCategories
                INNER JOIN CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
                INNER JOIN CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
            WHERE CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd 
                AND  CF_TransactionDetails.CategoryID <>2
                AND CF_TransactionDetailCategories.IsHandPayment= 0  
                AND CF_TransactionDetailCategories.IsJackpot = 0 
                AND CF_TransactionDetailCategories.IsPromo = 0 
                AND CF_Transactions.IsVoid <> 1
        ), 0
    )    

    --set @TotalSesiones =( Select  Count  (IDPlayerSession) From MK_PlayerSessionHistory 
    --where  ( MK_PlayerSessionHistory.DateTimeStart >= @DateTimeStart AND  MK_PlayerSessionHistory.DateTimeEnd<= @DateTimeEnd))
    --- Comentado momentaniamente hasta solvetar el problema del registro de impuesto en retiros paciales  en el player account 

    --set @PlayerAccountIN (SELECT  SUM(vw_PlayerAccountMovements.AmountIN)  
    --             FROM vw_PlayerAccountMovements
    --             WHERE (vw_PlayerAccountMovements.[DateTime] >= @DateTimeStart AND vw_PlayerAccountMovements.[DateTime] <= @DateTimeEnd AND ) )
    --set @PlayerAccountOUT (SELECT  SUM(vw_PlayerAccountMovements.AmountOUT)  
    --             FROM vw_PlayerAccountMovements
    --             WHERE (vw_PlayerAccountMovements.[DateTime] >= @DateTimeStart AND vw_PlayerAccountMovements.[DateTime] <= @DateTimeEnd AND ) )
    --SET @AmountInPlayerAccount = @AmountPlayerAccountVariation
    --SET @TotalSesiones = @AmountPlayerAccountVariation --- momentariamente el @@AmountPlayerAccountVariation el reporte lo carga en el campo  @TotalSesiones , no pude modificar el reporte 

	-- Uso Momentanio: 
    SET @MontoCajasQueNoAfectanCuentaPlayer = ISNULL
    (
        (
            SELECT SUM (CF_TransactionDetails.AmountIn)
            FROM CF_TransactionDetailCategories
                INNER JOIN  CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
                INNER JOIN   CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
            WHERE  CF_Transactions.[Time] >= @DateTimeStart AND CF_Transactions.[Time] <= @DateTimeEnd
                AND CF_Transactions.isvoid = 0 
                AND  CF_TransactionDetails.CategoryID = 19 
        ), 0
    )                                                    

    Set @TotalHP = ISNULL
    (
        (
            Select Sum (Amount)
            From  CF_HandPayments HP 
            Where (HP.[DateTime] >= @DateTimeStart AND HP.[DateTime] <= @DateTimeEnd)
        ), 0
    )
        
    Set @TotalHP = @TotalHP + ISNULL((Select Sum (Amount)
    From  [CF_Jackpots] JP 
    Where (JP.[DateTime] >= @DateTimeStart AND JP.[DateTime] <= @DateTimeEnd)),0)    
            
    -- - @MontoCajasQueNoAfectanCuentaPlayer 
    SET @PlayerAccountIN =  @AmountIn - @HardSoftCountBill -  @MontoCajasQueNoAfectanCuentaPlayer + @TotalHP  + ISNULL
    (
        (
            SELECT  SUM(vw_PlayerAccountMovements.AmountIN)  
            FROM vw_PlayerAccountMovements
            WHERE   vw_PlayerAccountMovements.IsVoid = 0 AND IDGM >= 0 
                AND  vw_PlayerAccountMovements.[DateTime] >= @DateTimeStart AND vw_PlayerAccountMovements.[DateTime] <= @DateTimeEnd 
        ), 0
    )

    SET @PlayerAccountOUT = @AmountOut+ @Taxes + ISNULL
    (
        (
            SELECT  SUM(vw_PlayerAccountMovements.AmountOUT)  
            FROM vw_PlayerAccountMovements
            WHERE vw_PlayerAccountMovements.IsVoid = 0 AND IDGM >= 0 
                AND  vw_PlayerAccountMovements.[DateTime] >= @DateTimeStart AND vw_PlayerAccountMovements.[DateTime] <= @DateTimeEnd
        ), 0 
    )
 
    --SET @PlayerAccountOUT = @AmountOutWihtTax + ISNULL((SELECT  SUM(vw_PlayerAccountMovements.AmountOUT)  
    -- FROM vw_PlayerAccountMovements
    -- WHERE IDGM >= 0 AND  vw_PlayerAccountMovements.[DateTime] >= @DateTimeStart AND vw_PlayerAccountMovements.[DateTime] <= @DateTimeEnd),0 )
    
    SET @AmountPlayerAccountVariation =   @PlayerAccountIN - @PlayerAccountOUT 
     
    --SET @AmountInPlayerAccount = @AmountPlayerAccountVariation
    --SET @TotalSesiones = @AmountPlayerAccountVariation --- momentariamente el @@AmountPlayerAccountVariation el reporte lo carga en el campo  @TotalSesiones , no pude modificar el reporte 

    ------------------------------------------------------------------------
    
    SET @AmountInPlayerAccount = 
	(
        SELECT SUM(Balance)
        FROM dbo.CF_PlayerAccounts 
        WHERE IsFrozen = 0 and IsClosed = 0
    )

    SET @TotalSesiones = @AmountInPlayerAccount --- momentariamente el @@AmountPlayerAccountVariation el reporte lo carga en el campo  @TotalSesiones , no pude modificar el reporte 

     -------------------------------------------------------------
    SET @Promociones = ISNULL
    (
        (
            SELECT SUM(Amount)
            FROM dbo.MKG_PromotionalCredits 
            WHERE  IDUserCreate IS NOT NULL AND DateTimeAssigned >= @DateTimeStart AND DateTimeAssigned <= @DateTimeEnd
        ), 0
    )
    
    ---PROMOCIONES CANCELADAS BY CASH OUT 
    SET @CanceledPromo = ISNULL
    (
        (
            SELECT SUM(Amount)
            FROM dbo.MKG_PromotionalCredits 
            WHERE  ([IDPaymentState] = 3 OR [IDPaymentState] = 4) 
                AND ( [IDGMInstancePaid] IS NULL AND DateTimePaid >= @DateTimeStart AND DateTimePaid <= @DateTimeEnd)
        ), 0
    )
    
    SET @NamePromo20 = 'Promociones Otorgadas CF (Uso de Instalaciones)'
    SET @TotalSlots = 
    (
        SELECT Count(Distinct IDGM) from dbo.SL_GMInstances 
        WHERE  (SL_GMInstances.DateTimeEnd >=  @DateTimeStart  OR SL_GMInstances.BDEnd = '')
            AND (SL_GMInstances.DateTimeStart <=  @DateTimeEnd)  
    )

    SET @TotalSlotsOcuped = 
    (
        SELECT COUNT (DISTINCT SL_GMInstances.IDGM)
        FROM MK_PlayerSessionHistory 
			INNER JOIN  SL_GMInstances ON  SL_GMInstances.IDGMInstance = MK_PlayerSessionHistory.IDGMInstance
        WHERE (MK_PlayerSessionHistory.DateTimeStart >= @DateTimeStart AND  MK_PlayerSessionHistory.DateTimeEnd<= @DateTimeEnd)
    )
END
 
SELECT 
        @AmountIn AS AmountIn, 
        @AmountOut AS AmountOut ,
        @Taxes AS Taxes, 
        @TotalSesiones AS TotalSesiones,
        @OcupacionActual AS OcupacionActual,
        @OcupacionPeriodo AS OcupacionPeriodo , 
        @Promociones AS Promociones,
    --    @AmountInPlayerAccount AS AmountInPlayerAccount,
        @EspecialPromo as EspecialPromo,
        @CanceledPromo as CanceledPromo,
        @NamePromo20 as NamePromo20,
        @TotalHP as TotalHP,
        @TotalJP as TotalJP,
        @TotalSlots as TotalSlots,
        @TotalSlotsOcuped as TotalSlotsOcuped,
        @AmountPlayerAccountVariation as AmountPlayerAccountVariation,
        @PlayerAccountIN as PlayerAccountIN,
        @PlayerAccountOUT as PlayerAccountOUT

GO



/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_CF_ResumenOperation2]
   Description:  Copied from RPT_CF_ResumenOperation
   AUTOR:        FrankZhang      FECHA :  15/03/2016 
  ------------------------------------------------------------ */
IF OBJECT_ID('GBI_RPT_CF_ResumenOperation2', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_CF_ResumenOperation2'
	DROP PROC GBI_RPT_CF_ResumenOperation2
END

PRINT N'CREATE PROC GBI_RPT_CF_ResumenOperation2'
GO

CREATE   PROCEDURE [dbo].[GBI_RPT_CF_ResumenOperation2]
    @DateTimeStart datetime, @DateTimeEnd datetime
AS
BEGIN

    DECLARE @results TABLE 
    (
        [Description] [varchar](50) NULL,
        IDGMManufacturer [tinyint] NOT NULL, 
        [CashOut] [decimal](18, 2) NOT NULL,
        [CashIN] [decimal](18, 2) NOT NULL, 
        [Sesiones] [bigint] NULL,
        [CantPlayer] [bigint] NOT NULL,
        [HandPayments] [decimal](18, 2) NOT NULL,
        [PROMO_IN] [decimal](18, 2) NOT NULL,
        [PROMO_OUT] [decimal](18, 2) NOT NULL    
    )

    INSERT INTO @results
        SELECT
            MF.[Description], 
            MF.IDGMManufacturer, 
            --800  AS CashOut,
            CashOut = SUM(vw_PlayerAccountMovements.AmountIN), 
            CashIN = SUM(vw_PlayerAccountMovements.AmountOUT) ,                  
            Sesiones = COUNT(IDPlayerAccount),
            --45 as CantPlayer,
            CantPlayer = COUNT(DISTINCT IDPlayerAccount),
        
            --Sum ( vw_PlayerAccountMovements.Balance) AS Ocupation   
            --HandPayments
            HandPayments = 
				ISNULL
				(
					(
						Select Sum (Amount)
						From  CF_HandPayments HP 
						Where (HP.[DateTime] >= @DateTimeStart AND HP.[DateTime] <= @DateTimeEnd)
							AND HP.IDGMFriendly in ( Select SG.IDGMFriendly From SL_GMs  SG where SG.IDGMManufacturer = MF.IDGMManufacturer)
					), 0
				)         
				+     
				ISNULL
				(
					(
						Select Sum (Amount)
						From  CF_Jackpots JP 
						Where (JP.[DateTime] >= @DateTimeStart AND JP.[DateTime] <= @DateTimeEnd)
							AND JP.IDGMFriendly in ( Select SG.IDGMFriendly From SL_GMs  SG where SG.IDGMManufacturer = MF.IDGMManufacturer)
					), 0
				),

            --- Promociones :: 
            --- Promo IN -- el correcto es DateTimePaid
			PROMO_IN = 
				ISNULL
				(
					(
						Select  Sum (Amount)
						From dbo.MKG_PromotionalCredits 
						Where (MKG_PromotionalCredits.DateTimePaid >= @DateTimeStart AND MKG_PromotionalCredits.DateTimePaid <= @DateTimeEnd)
							AND MKG_PromotionalCredits.IDGMInstancePaid IN 
							(
								Select MKG_PromotionalCredits.IDGMInstancePaid
								From MKG_PromotionalCredits
									INNER JOIN  SL_GMInstances ON SL_GMInstances.IDGMInstance = MKG_PromotionalCredits.IDGMInstancePaid 
									INNER JOIN SL_GMs ON SL_GMInstances.IDGM = SL_GMs.IDGM 
									INNER JOIN SL_GMManufacturers ON SL_GMs.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
								Where MF.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
							)
					), 0
				),

            --- PROMO OUT
			PROMO_OUT = 
				ISNULL
				(
					(
						Select  Sum (Amount)
						From dbo.MKG_PromotionalCredits 
						Where (MKG_PromotionalCredits.DateTimeAssigned >= @DateTimeStart AND MKG_PromotionalCredits.DateTimeAssigned <= @DateTimeEnd)
							AND MKG_PromotionalCredits.IDGMInstanceCreate IN 
							(
								Select MKG_PromotionalCredits.IDGMInstanceCreate 
								From MKG_PromotionalCredits
									INNER JOIN  SL_GMInstances ON SL_GMInstances.IDGMInstance = MKG_PromotionalCredits.IDGMInstanceCreate 
									INNER JOIN SL_GMs ON SL_GMInstances.IDGM = SL_GMs.IDGM 
									INNER JOIN SL_GMManufacturers ON SL_GMs.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
								Where MF.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
							)
					), 0
				) 
        FROM vw_PlayerAccountMovements
            INNER JOIN SL_GMs ON vw_PlayerAccountMovements.IDGM = SL_GMs.IDGM 
            INNER JOIN SL_GMManufacturers as MF ON SL_GMs.IDGMManufacturer = MF.IDGMManufacturer
        WHERE (vw_PlayerAccountMovements.[DateTime] >= @DateTimeStart AND vw_PlayerAccountMovements.[DateTime] <= @DateTimeEnd)
            AND vw_PlayerAccountMovements.IDGM >= 0 
        GROUP BY MF.[Description],MF.IDGMManufacturer
        ORDER by MF.[Description],MF.IDGMManufacturer
 
    INSERT Into @results
        SELECT 
            SL_GMManufacturers.Description, 
            SL_GMManufacturers.IDGMManufacturer, 
            [CashOut] = 0,
            [CashIN] = SUM (CF_HardSoftCountGMs.TotalAmount),
            0,0,0,0,0
        FROM  CF_HardSoftCount 
            INNER JOIN CF_HardSoftCountGMs ON CF_HardSoftCount.IDHSCount = CF_HardSoftCountGMs.IDHSCount
            INNER JOIN SL_GMInstances ON CF_HardSoftCountGMs.IDGMInstance = SL_GMInstances.IDGMInstance 
            INNER JOIN SL_GMs ON SL_GMInstances.IDGM = SL_GMs.IDGM 
            INNER JOIN SL_GMManufacturers ON SL_GMs.IDGMManufacturer = SL_GMManufacturers.IDGMManufacturer
        Where  CF_HardSoftCount.DateTimeStart >= @DateTimeStart AND CF_HardSoftCount.DateTimeStart <= @DateTimeEnd
        Group by SL_GMManufacturers.IDGMManufacturer, SL_GMManufacturers.Description


    SELECT 
        R.[Description],
        R.IDGMManufacturer,
        [CashOut]  = Sum (R.[CashOut]) ,
        [CashIN]   = Sum (R.[CashIN]),
        [Sesiones] = Sum (R.[Sesiones]),
        [CantPlayer] = Sum (R.[CantPlayer]),
        [HandPayments] = Sum (R.[HandPayments]),
        [PROMO_IN]  = Sum (R.[PROMO_IN]),
        [PROMO_OUT] = Sum (R.[PROMO_OUT]) 
    FROM @results as R
    GROUP by  R.[Description], R.IDGMManufacturer
    ORDER by  R.[Description], R.IDGMManufacturer
END

GO


/* ------------------------------------------------------------
   PROCEDURE:    [GBI_RPT_CF_ResumenOperation_Trend]
   Description:  To get Trend of NetWin and Win Slots for last 7 days
   @DefaultStartTime: 07:00:00
   @DefaultEndTime: 06:59:59
   AUTOR:        FrankZhang      FECHA :  27/11/2015 
  ------------------------------------------------------------ */
IF OBJECT_ID('RPT_CF_ResumenOperation_Trend', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC RPT_CF_ResumenOperation_Trend'
	DROP PROC RPT_CF_ResumenOperation_Trend
END

IF OBJECT_ID('GBI_RPT_CF_ResumenOperation_Trend', 'P') IS NOT NULL
BEGIN
	PRINT N'DROP PROC GBI_RPT_CF_ResumenOperation_Trend'
	DROP PROC GBI_RPT_CF_ResumenOperation_Trend
END

PRINT N'CREATE PROC GBI_RPT_CF_ResumenOperation_Trend'
GO

CREATE PROCEDURE GBI_RPT_CF_ResumenOperation_Trend
    @Day datetime, @DefaultStartTime char(8), @DefaultEndTime char(8), @num int /*Number of days*/
AS
BEGIN
    DECLARE @TotalDaySeconds int = 24*60*60;
    DECLARE  @StartTime datetime = DateAdd(day, - @num, CONVERT(date, @Day));
    SET @StartTime = DateAdd(hour, CONVERT(int, Left(@DefaultStartTime,2)), @StartTime);
    SET @StartTime = DateAdd(minute, CONVERT(int, Substring(@DefaultStartTime,4, 2)), @StartTime);
    SET @StartTime = DateAdd(second, CONVERT(int, Substring(@DefaultStartTime,7, 2)), @StartTime);

    DECLARE  @EndTime datetime = CONVERT(date, @Day);
    DECLARE @TotalEndSeconds int = Datepart(hour , @DefaultEndTime)*60*60 + Datepart(minute , @DefaultEndTime)*60 + Datepart(second , @DefaultEndTime);
    SET @EndTime = DateAdd(second, @TotalEndSeconds, @EndTime);

    DECLARE @GBI_RPT_CF_ROTrend TABLE
    (
        StartDateTime datetime, 
        DayDifference int
    )
    /*Applying table variables to improve query performance greatly*/
    INSERT INTO @GBI_RPT_CF_ROTrend Values ( @StartTime, @num - 1 );

    DECLARE @loop int = 1;
    WHILE (@loop < @num)
    BEGIN
        Insert into @GBI_RPT_CF_ROTrend Values( DateAdd(Day, @loop, @StartTime),  @num - 1 - @loop);
        SET @loop = @loop + 1;
    END

    /*Get HardSoftCountBill Mostly is null*/ 
    DECLARE @HardSoftCountBill TABLE
    (
        DayDifference int,
        HardSoftCountBill decimal(18,2)
    )  

    INSERT INTO @HardSoftCountBill(HardSoftCountBill, DayDifference)
        SELECT  HardSoftCountBill = SUM (CF_HardSoftCountGMs.TotalAmount),    
            DayDifference = DateDiff(second, CF_HardSoftCount.DateTimeStart, @EndTime) / @TotalDaySeconds 
        FROM CF_HardSoftCount 
            INNER JOIN CF_HardSoftCountGMs ON CF_HardSoftCount.IDHSCount = CF_HardSoftCountGMs.IDHSCount      
        WHERE  CF_HardSoftCount.DateTimeStart >= @StartTime AND CF_HardSoftCount.DateTimeStart <= @EndTime
        GROUP BY DateDiff(second, CF_HardSoftCount.DateTimeStart, @EndTime) / @TotalDaySeconds  

    /*Get AmountIn*/
    DECLARE @AmountInOut TABLE
    (
        DayDifference int,
        AmountIn decimal(18,2),
		AmountOut decimal(18,2)
    )

    INSERT INTO @AmountInOut(AmountIn, AmountOut, DayDifference)
        SELECT AmountIn = SUM (CF_TransactionDetails.AmountIn), AmountOut = SUM (CF_TransactionDetails.AmountOut), 
            DayDifference = DateDiff(second,  CF_Transactions.[Time], @EndTime) / @TotalDaySeconds
        FROM CF_TransactionDetailCategories
            INNER JOIN  CF_TransactionDetails ON CF_TransactionDetailCategories.CategoryID = CF_TransactionDetails.CategoryID
            INNER JOIN   CF_Transactions ON CF_TransactionDetails.TransactionID = CF_Transactions.TransactionID
        WHERE  CF_Transactions.[Time] >= @StartTime AND CF_Transactions.[Time] <= @EndTime
             AND  CF_TransactionDetails.CategoryID <>2
             AND CF_TransactionDetailCategories.IsHandPayment= 0 
             AND CF_TransactionDetailCategories.IsJackpot = 0 
             AND CF_TransactionDetailCategories.IsPromo = 0 
             AND CF_Transactions.IsVoid <> 1
        GROUP BY DateDiff(second,  CF_Transactions.[Time], @EndTime) / @TotalDaySeconds

    /*Get Taxes*/
    DECLARE @Taxes TABLE
    (
        DayDifference int,
        Taxes decimal(18,2)
    )

    INSERT INTO @Taxes(Taxes, DayDifference)
        SELECT Taxes = SUM(CF_TaxTransactionDetails.Amount), 
            DayDifference = DateDiff(second,  CF_Transactions.[Time], @EndTime) / @TotalDaySeconds
        FROM CF_Taxes 
            INNER JOIN CF_TaxTransactionDetails ON CF_TaxTransactionDetails.taxID = CF_Taxes.TaxID 
            INNER JOIN CF_Transactions ON CF_TaxTransactionDetails.TransactionID = CF_Transactions.TransactionID    
        WHERE CF_Transactions.[Time] >= @StartTime AND CF_Transactions.[Time] <= @EndTime AND CF_Transactions.IsVoid <> 1
        GROUP BY DateDiff(second,  CF_Transactions.[Time], @EndTime) / @TotalDaySeconds

    /*Get HandPayments*/
    DECLARE @HandPayments TABLE
    (
        DayDifference int,
        HandPayments decimal(18,2)
    )

    INSERT INTO @HandPayments(HandPayments, DayDifference)
        SELECT HandPayments = Sum (HP.Amount), 
            DayDifference = DateDiff(second,  HP.[DateTime], @EndTime) / @TotalDaySeconds
        FROM  CF_HandPayments HP 
            inner join  SL_GMs SG on SG.IDGMFriendly = HP.IDGMFriendly
        WHERE (HP.[DateTime] >= @StartTime AND HP.[DateTime] <= @EndTime)
        GROUP BY DateDiff(second,  HP.[DateTime], @EndTime) / @TotalDaySeconds

    /*Get Jackpots*/
    DECLARE @Jackpots TABLE
    (
        DayDifference int,
        Jackpots decimal(18,2)
    )

    INSERT INTO @Jackpots(Jackpots, DayDifference)                
        SELECT Jackpots = Sum (Amount), 
            DayDifference = DateDiff(second,  JP.[DateTime], @EndTime) / @TotalDaySeconds
        FROM  CF_Jackpots JP 
            INNER JOIN  SL_GMs SG on SG.IDGMFriendly = JP.IDGMFriendly
        WHERE (JP.[DateTime] >= @StartTime AND JP.[DateTime] <= @EndTime)
        GROUP BY DateDiff(second,  JP.[DateTime], @EndTime) / @TotalDaySeconds

    /*Get CashIn and CashOut*/
    DECLARE @CashInOut TABLE
    (
        DayDifference int,
        CashIn decimal(18,2),
        CashOut decimal(18,2)
    )

    INSERT INTO @CashInOut(CashIn, CashOut, DayDifference)    
        SELECT
            --COUNT(IDPlayerAccount) AS Sesiones,
            --COUNT(DISTINCT IDPlayerAccount) AS CantPlayer,
            CashIn  = SUM(pm.AmountOUT),    
            CashOut = SUM(pm.AmountIN),
            DayDifference = DateDiff(second,  pm.[DateTime], @EndTime) / @TotalDaySeconds
        FROM vw_PlayerAccountMovements pm
            INNER JOIN SL_GMs ON pm.IDGM = SL_GMs.IDGM 
            INNER JOIN SL_GMManufacturers as MF ON SL_GMs.IDGMManufacturer = MF.IDGMManufacturer
        WHERE (pm.[DateTime] >= @StartTime AND pm.[DateTime] <= @EndTime) AND pm.IDGM >= 0 
        GROUP BY DateDiff(second,  pm.[DateTime], @EndTime) / @TotalDaySeconds

    /*Get Final Result*/
    SELECT 
        tm.StartDateTime
        , tm.DayDifference
        , HardSoftCountBill
        , AmountIn
        , AmountOut
        , Taxes
        , HandPayments
        , Jackpots
        , CashIn
        , CashOut
    FROM @GBI_RPT_CF_ROTrend tm
        Left Join @HardSoftCountBill hscb on hscb.DayDifference = tm.DayDifference
        Left Join @AmountInOut amin on amin.DayDifference = tm.DayDifference
        Left Join @Taxes as tax on tax.DayDifference = tm.DayDifference
        Left Join @HandPayments as hp on hp.DayDifference = tm.DayDifference
        Left Join @Jackpots as jp on jp.DayDifference = tm.DayDifference
        Left Join @CashInOut as cash on cash.DayDifference = tm.DayDifference
    ORDER BY tm.StartDateTime
END
GO

-- ================================================
-- CFG_DashboardCharts
-- ================================================
IF Exists(Select * From INFORMATION_SCHEMA.TABLES Where  TABLE_TYPE='BASE TABLE' AND TABLE_NAME = 'CFG_DashboardCharts')
BEGIN
	PRINT N'DROP TABLE CFG_DashboardCharts'
    DROP TABLE CFG_DashboardCharts
END

GO

IF Exists(Select * From INFORMATION_SCHEMA.TABLES Where  TABLE_TYPE='BASE TABLE' AND TABLE_NAME = 'GBI_CFG_DashboardCharts')
BEGIN
	PRINT N'DROP TABLE GBI_CFG_DashboardCharts'
    DROP TABLE GBI_CFG_DashboardCharts
END
GO

CREATE TABLE [dbo].GBI_CFG_DashboardCharts
(
    [ID] INT           IDENTITY (1, 1) NOT NULL,
    [DisplayOrder]      INT           NOT NULL,
    [ChartName]         NVARCHAR (50)    NOT NULL,
    [ChartGroupName]    NVARCHAR (50) NULL,
    [ChartType]         NVARCHAR (50) NULL,
    [AddedToDashboard]  BIT           DEFAULT ((0)) NOT NULL,
    [ChartIcon]         NVARCHAR(50) NULL, 
    [ChartBgColor]      NVARCHAR(25) NULL, 
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

INSERT INTO [dbo].GBI_CFG_DashboardCharts
(
	[DisplayOrder], [ChartName], [ChartGroupName], [ChartType], [AddedToDashboard],[ChartIcon],[ChartBgColor]
) 
VALUES
	(1, 'TotalHandle','SlotOccupation','Number',1, 'ion-android-hand', 'bg-aqua'),
	(2, 'TotalWin','SlotOccupation','Number',1, 'ion-social-usd-outline', 'bg-green'),
	(3, 'TotalVisitors','SlotOccupation','Number',1, 'ion-ios-people', 'bg-yellow'),
	(4, 'TotalSlotOccupied','SlotOccupation','Number',1, 'ion-pie-graph', 'bg-yellow'),
	(5, 'TotalSlots','SlotOccupation','Number',1, 'ion-ios-monitor', 'bg-aqua'),
	(6, 'SlotsOccupationRate','SlotOccupation','Number',1, 'ion-ios-calculator-outline', 'bg-red'),
	(7, 'SlotsOccupationRateIndicator','SlotOccupation','Indicator',1, null, null),
	(8, 'VisitorsPerHourChart','SlotOccupation','Chart',1, null, null),
	(9, 'SlotsPerHourChart','SlotOccupation','Chart',1, null, null),
	(10, 'SessionsPerHourChart','SlotOccupation','Chart',1, null, null),
	(11, 'OccupationRatePerHourChart','SlotOccupation','Chart',1, null, null),
	(12, 'HandlesPerHourChart','SlotOccupation','Chart',1, null, null),
	(13, 'WinLossPerHourChart','SlotOccupation','Chart',1, null, null),
	(14, 'SlotOccupationTable','SlotOccupation','Table',1, null, null),

	(20, 'NetWin','OperationSummary','Number',1, 'ion-social-usd-outline', 'bg-green'),
	(21, 'PlayerCashInOut','OperationSummary','Number',1, 'ion-ios-people', 'bg-aqua'),
	(22, 'Taxes','OperationSummary','Number',1, 'ion-locked', 'bg-yellow'),
	(23, 'NetWinAfterTax','OperationSummary','Number',1, 'ion-ios-star', 'bg-red'),
	(24, 'WinSlots','OperationSummary','Number',1, 'ion-ios-loop-strong', 'bg-red'),
	(25, 'Promotion','OperationSummary','Indicator',1, 'ion-email', 'bg-green'),
	(26, 'Occupation','OperationSummary','Indicator',1, 'ion-pie-graph', 'bg-yellow'),

	(28, 'PromotionDonut','OperationSummary','Indicator',1, null, null),
	(29, 'SlotsOccupationRateDonut','OperationSummary','Number',1, null, null),
	(30, 'ProviderTable','OperationSummary','Table',1, null, null),

	(40, 'TopPlayers','Marketing','Table',1, null, null)
GO

/* Delete GenuinaBI Menu/Menu Item data */
BEGIN
	declare @idapp int
	declare @appname varchar(100) = 'Genuina_Dashboard'
	begin try
		begin transaction

		select @idapp = IDApp from CFG_Apps where Description = @appname

		delete from [CFG_AppMenuItemsTranslation] where [IDMenuItem] in (select a.IDMenuItem from [CFG_AppMenuItems] a, [CFG_AppMenues] b where a.IDMenuHeader = b.IDMenuHeader and b.IDApp = @idapp)
		delete from [CFG_AppMenuItemsAuthorizations] where [IDMenuItem] in (select a.IDMenuItem from [CFG_AppMenuItems] a, [CFG_AppMenues] b where a.IDMenuHeader = b.IDMenuHeader and b.IDApp = @idapp)
		delete from [CFG_AppMenuItems] where [IDMenuHeader] in (select [IDMenuHeader] from [CFG_AppMenues] where IDApp = @idapp)

		delete from [CFG_AppMenuesTranslation] where [IDMenuHeader] in (select [IDMenuHeader] from [CFG_AppMenues] where [IDApp] = @idapp)
		delete from [CFG_AppMenuesAuthorizations] where [IDMenuHeader] in (select [IDMenuHeader] from [CFG_AppMenues] where [IDApp] = @idapp)
		delete from [CFG_AppMenues] where [IDApp] = @idapp
	
		delete from [CFG_AppAuthorizations] where [IDApp] = @idapp
		delete from [CFG_Apps] where [IDApp] = @idapp

		commit transaction
	end try
	begin catch
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
		rollback transaction
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
	end catch
END
GO

/* Insert GenuinaBI Menu/Menu Items and assign permission to Administrators user group */
declare @idapp int
declare @idusergroup int
declare @idmenuheader int
declare @idmenuitem int
declare @appname varchar(100) = 'Genuina_Dashboard' /*App Name*/
declare @appdescription varchar(100) = 'Dashboard'  /*Controller Name*/
begin try
	select @idapp = max(IDApp) + 1 from CFG_Apps
	select @idusergroup = u.IDUserGroup from CFG_UserGroups u where u.[Description]='Administradores'

	begin transaction
		/* Insert App - first menu level */
		insert into [CFG_Apps] ([IDApp], [Description], [AssemblyPath], [AssemblyType], [CanEditAuthorizations]) values (@idapp, @appname, '', 'Web Platform', 1)
		insert into [CFG_AppAuthorizations] ([IDApp], [IDUserGroup]) values (@idapp, @idusergroup)
		/* Insert AppMenu - second menu level */
		insert into [CFG_AppMenues] ([IDApp], [Order], [Enabled]) values (@idapp, 1, 1)
		set @idmenuheader = SCOPE_IDENTITY()

		/* Insert AppMenu translations */
		insert into [CFG_AppMenuesTranslation] (IDMenuHeader, IDLanguage, Description, Tooltip, Notes) values (@idmenuheader, 'es ', 'Dashboard', '', '')
		insert into [CFG_AppMenuesTranslation] (IDMenuHeader, IDLanguage, Description, Tooltip, Notes) values (@idmenuheader, 'en ', 'Dashboard', '', '')
		insert into [CFG_AppMenuesTranslation] (IDMenuHeader, IDLanguage, Description, Tooltip, Notes) values (@idmenuheader, 'fr ', 'Dashboard', '', '')

		/* Insert AppMenuesAuthorizations */
		insert into [CFG_AppMenuesAuthorizations] ([IDMenuHeader], [IDUserGroup]) values (@idmenuheader, @idusergroup)
		/* Insert AppMenuItems / dashboards - third menu level */
		insert into [CFG_AppMenuItems] ([IDMenuHeader], [AssemblyPath], [AssemblyType], [IsAssemblyPathAbsolute], [Order], [Enabled]) values (@idmenuheader, 'OperationSummary', @appdescription, 0, 0, 1) /*Action Name*/

		set @idmenuitem = SCOPE_IDENTITY()
		/* Insert AppMenuItem translations */
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'es ', 'Resumen Operación', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'en ', 'Operation Summary', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'fr ', 'Résumé d''Opération', '', '')

		/* Insert AppMenuItemsAuthorizations */
		insert into [CFG_AppMenuItemsAuthorizations] ([IDMenuItem], [IDUserGroup]) values (@idmenuitem, @idusergroup)

		insert into [CFG_AppMenuItems] ([IDMenuHeader], [AssemblyPath], [AssemblyType], [IsAssemblyPathAbsolute], [Order], [Enabled]) values (@idmenuheader, 'SlotOccupation', @appdescription, 0, 1, 1) /*Action Name*/
		set @idmenuitem = SCOPE_IDENTITY()
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'es ', 'Slot Ocupación', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'en ', 'Slot Occupation', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'fr ', 'Slot Occupation', '', '')

		/* Insert AppMenuItemsAuthorizations */
		insert into [CFG_AppMenuItemsAuthorizations] ([IDMenuItem], [IDUserGroup]) values (@idmenuitem, @idusergroup)

		insert into [CFG_AppMenuItems] ([IDMenuHeader], [AssemblyPath], [AssemblyType], [IsAssemblyPathAbsolute], [Order], [Enabled]) values (@idmenuheader, 'TopPlayers', @appdescription, 0, 2, 1) /*Action Name*/
		set @idmenuitem = SCOPE_IDENTITY()
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'es ', 'Mejores Jugadores', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'en ', 'Top Players', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'fr ', 'Meilleurs Joueurs', '', '')

		/* Insert AppMenuItemsAuthorizations */
		insert into [CFG_AppMenuItemsAuthorizations] ([IDMenuItem], [IDUserGroup]) values (@idmenuitem, @idusergroup)

		insert into [CFG_AppMenues] ([IDApp], [Order], [Enabled]) values (@idapp, 1, 1) /* Add Proactive MK */
		set @idmenuheader = SCOPE_IDENTITY()
		set @appdescription = 'Marketing' /*controller name*/
		/* Insert AppMenu translations */
		insert into [CFG_AppMenuesTranslation] (IDMenuHeader, IDLanguage, Description, Tooltip, Notes) values (@idmenuheader, 'es ', 'MK Proactivo', '', '')
		insert into [CFG_AppMenuesTranslation] (IDMenuHeader, IDLanguage, Description, Tooltip, Notes) values (@idmenuheader, 'en ', 'Proactive MK', '', '')
		insert into [CFG_AppMenuesTranslation] (IDMenuHeader, IDLanguage, Description, Tooltip, Notes) values (@idmenuheader, 'fr ', 'MK Proactive', '', '')

		/* Insert AppMenuesAuthorizations */
		insert into [CFG_AppMenuesAuthorizations] ([IDMenuHeader], [IDUserGroup]) values (@idmenuheader, @idusergroup)
		/* Insert AppMenuItems / dashboards - third menu level */
		insert into [CFG_AppMenuItems] ([IDMenuHeader], [AssemblyPath], [AssemblyType], [IsAssemblyPathAbsolute], [Order], [Enabled]) values (@idmenuheader, 'PlayerSearch', @appdescription, 0, 0, 1) /*Action Name*/

		set @idmenuitem = SCOPE_IDENTITY()
		/* Insert AppMenuItem translations */
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'es ', 'Busca Jugador', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'en ', 'Player Search', '', '')
		insert into [CFG_AppMenuItemsTranslation] ([IDMenuItem], [IDLanguage], [Description], [Tooltip], [Notes]) values (@idmenuitem, 'fr ', 'Joueur Recherche', '', '')

		/* Insert AppMenuItemsAuthorizations */
		insert into [CFG_AppMenuItemsAuthorizations] ([IDMenuItem], [IDUserGroup]) values (@idmenuitem, @idusergroup)

	commit transaction
end try
begin catch
	DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
	rollback transaction
	RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
end catch
GO




