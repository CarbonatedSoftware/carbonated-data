/* Creates tables and data for SQL test. */
USE [CarbonatedTest]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cities](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[city] [nvarchar](100) NULL,
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
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (1, N'New York', N'NY', 8405837)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (2, N'Los Angeles', N'CA', 3884307)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (3, N'Chicago', N'IL', 2718782)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (4, N'Houston', N'TX', 2195914)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (5, N'Philadelphia', N'PA', 1553165)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (6, N'Phoenix', N'AZ', 1513367)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (7, N'San Antonio', N'TX', 1409019)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (8, N'San Diego', N'CA', 1355896)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (9, N'Dallas', N'TX', 1257676)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (10, N'San Jose', N'CA', 998537)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (11, N'Austin', N'TX', 885400)
INSERT [dbo].[cities] ([id], [city], [state], [population]) VALUES (12, N'Indianapolis', N'IN', 843393)
SET IDENTITY_INSERT [dbo].[cities] OFF

SET IDENTITY_INSERT [dbo].[type_test] ON 
INSERT [dbo].[type_test] ([id], [bool], [byte], [short], [int], [long], [float], [double], [decimal], [datetime], [guid_as_string], [guid_as_uniqueid], [char], [string], [byte_array], [int_enum], [string_enum]) VALUES (1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[type_test] ([id], [bool], [byte], [short], [int], [long], [float], [double], [decimal], [datetime], [guid_as_string], [guid_as_uniqueid], [char], [string], [byte_array], [int_enum], [string_enum]) VALUES (2, 0, 0, 0, 0, 0, 0, 0, CAST(0.00 AS Decimal(10, 2)), CAST(N'2018-04-02T13:14:15.000' AS DateTime), N'', NULL, N'', N'', NULL, 0, N'')
INSERT [dbo].[type_test] ([id], [bool], [byte], [short], [int], [long], [float], [double], [decimal], [datetime], [guid_as_string], [guid_as_uniqueid], [char], [string], [byte_array], [int_enum], [string_enum]) VALUES (3, 1, 1, 2, 3, 5, 8.13, 21.34, CAST(55.89 AS Decimal(10, 2)), CAST(N'2018-04-02T13:14:15.000' AS DateTime), N'7ca43d15-6e87-49df-baff-db241d0d494c', N'7ca43d15-6e87-49df-baff-db241d0d494c', N'c', N'str', 0xFEDCBA9876543210, 3, N'Green')
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
