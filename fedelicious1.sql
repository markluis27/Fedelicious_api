CREATE PROCEDURE SP_minandmax
AS
BEGIN 
SELECT 
    c.full_name,
    MIN(o.order_id) AS MinOrder,
    MAX(o.order_id) AS MaxOrder
FROM orders o
INNER JOIN customers c 
    ON o.customer_id = c.customer_id
GROUP BY c.customer_id, c.full_name
ORDER BY MinOrder ASC;

END;
