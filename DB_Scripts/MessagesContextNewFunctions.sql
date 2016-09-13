CREATE FUNCTION GetTime
(
	@datetime DateTime
)
RETURNS VARCHAR(8)
AS
BEGIN
	IF @datetime IS NULL
		SET @time = '';
	ELSE
		SET @time = SUBSTRING(CONVERT(VARCHAR(25), @datetime, 131),12,5)+' '+SUBSTRING(CONVERT(VARCHAR(25), @datetime, 131),24,2);
	RETURN @time;
END
GO
