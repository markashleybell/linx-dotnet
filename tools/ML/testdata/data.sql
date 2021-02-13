USE gtdpad
GO

SELECT 
i.id, 
LTRIM(RTRIM(REPLACE(REPLACE(i.title, CHAR(13), ''), CHAR(10), ''))) AS title, 
LTRIM(RTRIM(i.body)) AS body, 
LTRIM(RTRIM(p.title)) as [page], 
LTRIM(RTRIM(l.title)) AS category
FROM items i
INNER JOIN lists l ON l.id = i.list_id AND l.deleted IS NULL
INNER JOIN pages p ON p.id = l.page_id AND p.deleted IS NULL
WHERE i.deleted IS NULL 
AND (i.body LIKE 'http://%' or i.body LIKE 'https://%')
OR (i.title LIKE 'http://%' or i.title LIKE 'https://%')
ORDER BY l.title, i.title
