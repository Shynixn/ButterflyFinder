WITH LINK(ID, name, parent_id) AS (
	SELECT ID, name, parent_id FROM SHY_FILECACHE WHERE LOWER(name) LIKE('%KEY%')
	UNION ALL
	SELECT SHY_FILECACHE.ID, IFNULL(SHY_FILECACHE.name || '/', '') || LINK.name, SHY_FILECACHE.parent_id
	FROM LINK INNER JOIN SHY_FILECACHE ON LINK.parent_id = SHY_FILECACHE.ID
	)
SELECT name FROM LINK WHERE name LIKE ('%:%') group by name order by CASE WHEN name Like('%exe') THEN 1 ELSE 2 END, name ASC LIMIT @param0;

