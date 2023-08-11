/* Creates tables and data for SQL test. */
USE [CarbonatedTest]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cities](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NULL,
	[state] [nchar](2) NULL,
	[population] [int] NULL,
 CONSTRAINT [PK_cities] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[type_test](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[bool] [bit] NULL,
	[byte] [tinyint] NULL,
	[short] [smallint] NULL,
	[int] [int] NULL,
	[long] [bigint] NULL,
	[float] [float] NULL,
	[double] [float] NULL,
	[decimal] [decimal](10, 2) NULL,
	[datetime] [datetime] NULL,
	[datetime2] [datetime2] NULL,
	[date] [date] NULL,
	[time] [time](7) NULL,
	[guid_as_string] [nvarchar](36) NULL,
	[guid_as_uniqueid] [uniqueidentifier] NULL,
	[char] [nvarchar](1) NULL,
	[string] [nvarchar](100) NULL,
	[byte_array] [varbinary](50) NULL,
	[int_enum] [int] NULL,
	[string_enum] [nvarchar](20) NULL,
 CONSTRAINT [PK_type_test] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[cities] ON 
INSERT [dbo].[cities] ([id], [name], [state], [population])
VALUES
	(1, N'New York', N'NY', 8405837),
	(2, N'Los Angeles', N'CA', 3884307),
	(3, N'Chicago', N'IL', 2718782),
	(4, N'Houston', N'TX', 2195914),
	(5, N'Philadelphia', N'PA', 1553165),
	(6, N'Phoenix', N'AZ', 1513367),
	(7, N'San Antonio', N'TX', 1409019),
	(8, N'San Diego', N'CA', 1355896),
	(9, N'Dallas', N'TX', 1257676),
	(10, N'San Jose', N'CA', 998537),
	(11, N'Austin', N'TX', 885400),
	(12, N'Indianapolis', N'IN', 843393);
SET IDENTITY_INSERT [dbo].[cities] OFF

SET IDENTITY_INSERT [dbo].[type_test] ON 
INSERT [dbo].[type_test] ([id], [bool], [byte], [short], [int], [long], [float], [double], [decimal], [datetime], [datetime2], [date], [time], [guid_as_string], [guid_as_uniqueid], [char], [string], [byte_array], [int_enum], [string_enum])
VALUES
	(1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
	(2, 0, 0, 0, 0, 0, 0, 0, CAST(0.00 AS Decimal(10, 2)), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'0001-01-01T00:00:00.000' AS DateTime2), CAST(N'0001-01-1' AS Date), CAST(N'00:00:00' AS Time), N'', NULL, N'', N'', NULL, 0, N''),
	(3, 1, 1, 2, 3, 5, 8.13, 21.34, CAST(55.89 AS Decimal(10, 2)), CAST(N'2018-04-02T13:14:15.000' AS DateTime), CAST(N'2023-08-11T14:09:15.000' AS DateTime2), CAST(N'2023-08-11' AS Date), CAST(N'14:09:15' AS Time), N'7ca43d15-6e87-49df-baff-db241d0d494c', N'7ca43d15-6e87-49df-baff-db241d0d494c', N'c', N'str', 0xFEDCBA9876543210, 3, N'Green');
SET IDENTITY_INSERT [dbo].[type_test] OFF
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCitiesByState 
	@state nchar(2)
AS
BEGIN
	SELECT * FROM cities WHERE [state] = @state
END
GO
