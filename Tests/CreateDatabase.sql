USE [master]
GO

IF (SELECT windows_sku FROM sys.dm_os_windows_info) IS NOT NULL
BEGIN
	EXEC sp_configure 'filestream access level',2
END
GO

RECONFIGURE WITH OVERRIDE
GO

DECLARE @DataFolderPath NVARCHAR(2000) = CONVERT(NVARCHAR(2000), SERVERPROPERTY('InstanceDefaultDataPath'))
DECLARE @MDFPath NVARCHAR(2000) = @DataFolderPath + 'Seal_Test.mdf';
DECLARE @FSHPath NVARCHAR(2000) = CASE WHEN (SELECT windows_sku FROM sys.dm_os_windows_info) IS NOT NULL THEN @DataFolderPath + 'Seal_Test.ndf' ELSE NULL END
DECLARE @LDFPath NVARCHAR(2000) = @DataFolderPath + 'Seal_Test.ldf';

DECLARE @sql NVARCHAR(MAX) = '

CREATE DATABASE [Seal_Test] 
    ON PRIMARY 
    ( 
        NAME = N''Seal_Test'', 
        FILENAME = N''' + @MDFPath + ''', 
        SIZE = 2048KB, 
        MAXSIZE = UNLIMITED, 
        FILEGROWTH = 1024KB 
    )
    
'

IF @FSHPath IS NOT NULL
BEGIN
	SET @sql = @sql + ',FILEGROUP [FileStreamContainer] CONTAINS FILESTREAM DEFAULT 
    ( 
        NAME = N''FileStreamHolder'', 
        FILENAME = N''' + @FSHPath + ''' 
    )'
END

SET @sql = @sql + 'LOG ON 
    ( 
        NAME = N''Seal_Test_log'', 
        FILENAME = N''' + @LDFPath + ''', 
        SIZE = 1024KB, 
        MAXSIZE = 2048GB, 
        FILEGROWTH = 10%
    )'

PRINT @sql
EXEC sp_executesql @sql

GO
ALTER DATABASE [Seal_Test] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Seal_Test].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Seal_Test] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [Seal_Test] SET ANSI_NULLS OFF
GO
ALTER DATABASE [Seal_Test] SET ANSI_PADDING OFF
GO
ALTER DATABASE [Seal_Test] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [Seal_Test] SET ARITHABORT OFF
GO
ALTER DATABASE [Seal_Test] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [Seal_Test] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [Seal_Test] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [Seal_Test] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [Seal_Test] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [Seal_Test] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [Seal_Test] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [Seal_Test] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [Seal_Test] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [Seal_Test] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [Seal_Test] SET  DISABLE_BROKER
GO
ALTER DATABASE [Seal_Test] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [Seal_Test] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [Seal_Test] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [Seal_Test] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [Seal_Test] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [Seal_Test] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [Seal_Test] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [Seal_Test] SET  READ_WRITE
GO
ALTER DATABASE [Seal_Test] SET RECOVERY FULL
GO
ALTER DATABASE [Seal_Test] SET  MULTI_USER
GO
ALTER DATABASE [Seal_Test] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [Seal_Test] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'Seal_Test', N'ON'
GO
USE [Seal_Test]
GO
/****** Object:  Schema [Sam]    Script Date: 01/30/2014 23:30:05 ******/
CREATE SCHEMA [Sam] AUTHORIZATION [dbo]
GO
/****** Object:  Schema [OMS]    Script Date: 01/30/2014 23:30:05 ******/
CREATE SCHEMA [OMS] AUTHORIZATION [dbo]
GO
/****** Object:  View [dbo].[TheView]    Script Date: 01/30/2014 23:30:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TheView]
AS

SELECT 'A' AS A, 4 AS B, GETDATE() AS C
UNION
SELECT NULL AS A, 4 AS B, GETDATE() AS C
UNION
SELECT 'A' AS A, 4 AS B, NULL AS C
GO
/****** Object:  StoredProcedure [OMS].[ShowMe]    Script Date: 01/30/2014 23:30:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OMS].[ShowMe]
AS
BEGIN

	SELECT 'trains' AS HeaderValue

	SELECT 
		CAST('ALKJFLKAJGKLJLKAJLVKAJSFKLAJDSFLKASDJFLKSDJFKLDSFJ' AS VARCHAR(1000)) AS VarCharValue,
		CAST(4500000 AS INT) AS Int32Value,
		CAST(450000000000000000 AS BIGINT) AS Int64Value,
		CAST(4500 AS SMALLINT) AS Int16Value,
		CAST(45 AS TINYINT) AS TinyValue,
		CAST(1 AS BIT) AS BooleanValue,
		CAST(45.92 AS DECIMAL(4,2)) AS NumericValue,
		CAST(GETDATE() AS DATETIME) AS RightNow
		
	SELECT 
		4 AS a,
		NEWID() AS b,
		'zorgons' AS c
		
	EXEC sp_helptext sp_helptext	
	
	SELECT * FROM sys.objects

END
GO
/****** Object:  Table [dbo].[Records]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF (SELECT windows_sku FROM sys.dm_os_windows_info) IS NOT NULL
BEGIN
	CREATE TABLE [dbo].[Records](
		[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
		[SerialNumber] [int] NULL,
		[Chart] [varbinary](max) FILESTREAM  NULL,
	UNIQUE NONCLUSTERED 
	(
		[SerialNumber] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	UNIQUE NONCLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] FILESTREAM_ON [FileStreamContainer]
END
GO
SET ANSI_PADDING OFF
GO
IF (SELECT windows_sku FROM sys.dm_os_windows_info) IS NOT NULL
BEGIN
	TRUNCATE TABLE Seal_Test.dbo.Records
	INSERT INTO Seal_Test.dbo.Records
		VALUES (newid(), 3, CAST ('Seismic Data' as varbinary(max))),
			   (newid(), 4, CAST (N'你能不能举个例子说明一下你刚才提到的那些进展情况？' as varbinary(max)));
END
GO
/****** Object:  StoredProcedure [dbo].[ListOnly]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ListOnly]
	@Kris INT
AS
BEGIN

	SELECT GETDATE() AS Times
	UNION 
	SELECT GETDATE()-1 
	UNION 
	SELECT GETDATE()-3

END
GO
/****** Object:  StoredProcedure [dbo].[InOutReturn]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InOutReturn]
	@Greg MONEY,
	@Tuila DATETIME2,
	@Samuel VARCHAR(90) = NULL,
	@Kings INT = NULL,
	@TheIt INT OUTPUT
AS
BEGIN

	SET @TheIt = 92;
	RETURN 5;

END
GO
/****** Object:  UserDefinedTableType [OMS].[tvp_Alma]    Script Date: 01/30/2014 23:30:07 ******/
CREATE TYPE [OMS].[tvp_Alma] AS TABLE(
	[Zamboi] [int] NULL,
	[TheThird] [smallmoney] NULL
)
GO
/****** Object:  Table [dbo].[TransactionTable]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionTable](
	[NumberValue] [int] NOT NULL,
	[TimestampValue] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[VarBinarySproc]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[VarBinarySproc]
	@InVar VARBINARY(1024)
AS
BEGIN

	SELECT 67 AS Id, @InVar AS ImageColumn;

END
GO
/****** Object:  StoredProcedure [Sam].[VanillaProc_v2]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Sam].[VanillaProc_v2] 
AS
BEGIN

	SELECT 
		CAST('ALKJFLKAJGKLJLKAJLVKAJSFKLAJDSFLKASDJFLKSDJFKLDSFJ' AS VARCHAR(1000)) AS VarCharValue,
		CAST(4500000 AS INT) AS Int32Value,
		CAST(450000000000000000 AS BIGINT) AS Int64Value,
		CAST(4500 AS SMALLINT) AS Int16Value,
		CAST(45 AS TINYINT) AS TinyValue,
		CAST(1 AS BIT) AS BooleanValue,
		CAST(45.92 AS DECIMAL(4,2)) AS NumericValue,
		CAST(GETDATE() AS DATETIME) AS RightNow,
		CAST(GETDATE() AS DATETIME2(7)) AS Sql8

END
GO
/****** Object:  StoredProcedure [dbo].[VanillaProc]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[VanillaProc] 
AS
BEGIN

	SELECT 
		CAST('ALKJFLKAJGKLJLKAJLVKAJSFKLAJDSFLKASDJFLKSDJFKLDSFJ' AS VARCHAR(1000)) AS VarCharValue,
		CAST(4500000 AS INT) AS Int32Value,
		CAST(450000000000000000 AS BIGINT) AS Int64Value,
		CAST(4500 AS SMALLINT) AS Int16Value,
		CAST(45 AS TINYINT) AS TinyValue,
		CAST(1 AS BIT) AS BooleanValue,
		CAST(45.92 AS DECIMAL(4,2)) AS NumericValue,
		CAST(GETDATE() AS DATETIME) AS RightNow

END
GO
/****** Object:  StoredProcedure [OMS].[FunnyNames]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OMS].[FunnyNames]
AS
BEGIN

	SELECT 
		4 AS a,
		NEWID() AS b,
		'zorgons' AS c

END
GO
/****** Object:  StoredProcedure [dbo].[ComplexList]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ComplexList]
	@Super TEXT
AS
BEGIN

	SELECT * FROM dbo.TheView
	
	RETURN 9;

END
GO
/****** Object:  StoredProcedure [OMS].[TvpTest]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [OMS].[TvpTest]
	@ParentId CHAR(40),
	@Helaman AS OMS.tvp_Alma READONLY
AS
BEGIN

	SELECT * FROM @Helaman

END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_TimeUpdate]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_TimeUpdate]
AS
BEGIN

	UPDATE dbo.TransactionTable
	SET TimestampValue = GETDATE()

END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_Increment]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_Increment]
	@NewValue INT
AS
BEGIN

	UPDATE dbo.TransactionTable
	SET NumberValue = @NewValue

END
GO
/****** Object:  StoredProcedure [dbo].[GetFileStreamExample]    Script Date: 01/30/2014 23:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetFileStreamExample]
AS
BEGIN
	
	SELECT 
		Id,
		SerialNumber,
		Chart.PathName() AS FilePath
	FROM dbo.Records
	WHERE SerialNumber = 3
	
END
GO

CREATE PROCEDURE [dbo].[GetFileStreamExampleUnicode]
AS
BEGIN
	
	SELECT 
		Id,
		SerialNumber,
		Chart.PathName() AS FilePath
	FROM dbo.Records
	WHERE SerialNumber = 4
	
END
GO



INSERT INTO [dbo].[TransactionTable]
           ([NumberValue]
           ,[TimestampValue])
     VALUES
           (40
           ,'5/25/2016 9:13:02 AM')
GO

CREATE OR ALTER PROCEDURE Sam.DoSomethingMan
AS
INSERT INTO dbo.TransactionTable ( NumberValue, TimestampValue )
VALUES ( -45, GETUTCDATE() );
GO

CREATE OR ALTER PROCEDURE Sam.GetSomethingMan
AS
SELECT 4 AS whatever
GO

CREATE OR ALTER PROCEDURE dbo.LongSprocIsLong
AS
BEGIN

    WAITFOR DELAY '00:00:05'

END
GO
