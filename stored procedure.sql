--AGGREGATE FUNCTIONS 
--STORED PROCEDURE 1 - minandmax
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
—-- csutomer activity
EXEC	SP_minandmax;

--STORED PROCEDURE 2 - countotalsold
CREATE PROCEDURE SP_countotalsold
AS
BEGIN
   SELECT 
    m.food_name, 
    SUM(oi.quantity) AS totalsold
FROM order_items oi
INNER JOIN menu_items m 
    ON oi.menu_id = m.menu_id
GROUP BY m.menu_id, m.food_name
ORDER BY totalsold DESC;
END; 
—- best seller

EXEC	SP_countotalsold;

--STORED PROCEDURE 3 - betweenorder
CREATE PROCEDURE SP_betweenorder
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN 
    SELECT 
        c.full_name,
        COUNT(o.order_id) AS total_orders
    FROM orders o
    INNER JOIN customers c 
        ON o.customer_id = c.customer_id
    WHERE o.order_date BETWEEN @StartDate AND @EndDate
    GROUP BY c.full_name
    ORDER BY total_orders DESC;
END;  
-— SALES REPORT


EXEC	SP_betweenorder; 

--STORED PROCEDURE 4 - totaluser
CREATE PROCEDURE SP_totaluser
AS
BEGIN
SELECT COUNT(*) AS totalcustomers
FROM customers;
END;
—- DASHBOARD

EXEC	SP_totaluser;





--STORED PROCEDURE 5 - weeklysalesreport
CREATE PROCEDURE SP_weeklysalesreport
    @TargetMonth INT,
    @TargetYear INT
AS
BEGIN 
    SELECT 
        YEAR(o.order_date) AS year,
        MONTH(o.order_date) AS month,
        ((DAY(o.order_date) - 1) / 7) + 1 AS week_of_month,
        SUM(o.total_amount) AS total_sales,
        COUNT(*) AS total_orders,
        COUNT(o.customer_id) AS total_customers
    FROM orders o
    INNER JOIN customers c 
        ON o.customer_id = c.customer_id
    WHERE MONTH(o.order_date) = @TargetMonth 
      AND YEAR(o.order_date) = @TargetYear
    GROUP BY 
        YEAR(o.order_date),
        MONTH(o.order_date),
        ((DAY(o.order_date) - 1) / 7) + 1
    ORDER BY 
        week_of_month DESC;
END; 
-–-SALES REPORT
EXEC	SP_weeklysalesreport;


--STORED PROCEDURE 6 - insertrevenue
CREATE PROCEDURE SP_insertrevenue
AS
BEGIN
    SELECT
    o.order_type, 
    COUNT(*) AS order_count, 
    SUM(o.total_amount) AS revenue
FROM orders o
GROUP BY o.order_type
ORDER BY revenue DESC;
END; – SALES REPORT
EXEC	SP_insertrevenue;

--STORED PROCEDURE 7 - monthsales
CREATE PROCEDURE SP_monthsales
    @TargetYear INT -- Idagdag ito
AS
BEGIN 
    SELECT 
        YEAR(o.order_date) AS year,
        MONTH(o.order_date) AS month,
        SUM(o.total_amount) AS total_sales,
        COUNT(*) AS total_orders,
        COUNT(o.customer_id) AS total_customers
    FROM orders o
    INNER JOIN customers c 
        ON o.customer_id = c.customer_id
    WHERE YEAR(o.order_date) = @TargetYear -- I-filter dito
    GROUP BY 
        YEAR(o.order_date),
        MONTH(o.order_date)
    ORDER BY month DESC;
END; — SALES REPORT

EXEC	SP_monthsales;

--STORED PROCEDURE 8 - summaryorders
CREATE PROCEDURE SP_summaryorders
@fullname nvarchar (100)
AS
BEGIN 
    SELECT 
        c.full_name,
        MAX(r.number_of_guests) AS largest_reservation,
        MIN(r.number_of_guests) AS smallest_reservation
    FROM reservations r
    INNER JOIN customers c ON r.customer_id = c.customer_id
    WHERE c.full_name = @fullname
    GROUP BY c.full_name;
END; —customer activivyt



END;
EXEC	SP_summaryorders;

--STORED PROCEDURE 9 - dailyrevenue
CREATE PROCEDURE SP_dailyrevenue
AS
BEGIN 
	SELECT
payment_date,
SUM(amount) AS daily_revenue
FROM payments
GROUP BY payment_date
ORDER BY payment_date DESC
END; – DASHBAORD
EXEC	SP_dailyrevenue;


--STORED PROCEDURE 10 - Listreservation
CREATE PROCEDURE SP_Listreservation
    @fullname NVARCHAR(100)
AS
BEGIN 
    SELECT 
        c.full_name,
        r.reservation_date,
        r.reservation_time,
        MAX(r.number_of_guests) AS max_guests, -- Kukunin ang pinakamaraming guests sa booking na iyon
        r.reservation_status
    FROM reservations r
    INNER JOIN customers c ON r.customer_id = c.customer_id
    WHERE c.full_name = @fullname -- Exact match, walang % 
    GROUP BY 
        c.full_name,
        r.reservation_date,
        r.reservation_time,
        r.reservation_status
    ORDER BY r.reservation_date DESC;
END; — CUSTOMER ACTIVITY



END;
EXEC	SP_Listreservation;

--STORED PROCEDURE 11 -  countreservation
CREATE PROCEDURE SP_countreservation
    @reservation_date DATE
AS
BEGIN
    SELECT 
        reservation_date,
        COUNT(*) AS totalreservations
    FROM reservations
    WHERE reservation_date = @reservation_date
    GROUP BY reservation_date;
END
END; — DASHBAORD
EXEC	SP_countreservation;

--STORED PROCEDURE 12 - yearlysummary
CREATE PROCEDURE SP_yearlysummary
    @year INT
AS
BEGIN
    SELECT
        YEAR(order_date) AS sales_year,
        SUM(total_amount) AS total_sales,
        COUNT(*) AS total_orders,
        COUNT(DISTINCT customer_id) AS total_customers
    FROM orders
    WHERE YEAR(order_date) = @year
    GROUP BY YEAR(order_date);
END; — SALES REPORT


--SUBQUERIES
--STORED PROCEDURE 13 - getCustomerReservations
CREATE PROCEDURE SP_getCustomerReservations
    @fullname NVARCHAR(100) = NULL -- Idinagdag para pwedeng i-filter
AS
BEGIN
    SELECT
        c.customer_id,
        c.full_name,
        c.email,
        r.reservation_id,
        r.reservation_date,
        r.reservation_time,
        r.downpayment_amount,
        (SELECT a.full_name
         FROM admins a
         WHERE a.admin_id = r.confirmed_by_admin_id) AS confirmed_by_admin,
        (SELECT COUNT(*)
         FROM reservations r3
         WHERE r3.customer_id = c.customer_id
         AND r3.reservation_status = 'Confirmed') AS confirmed_reservations,
        (SELECT COUNT(*)
         FROM reservations r4
         WHERE r4.customer_id = c.customer_id
         AND r4.reservation_status = 'Cancelled') AS cancelled_reservations,
        (SELECT SUM(r5.number_of_guests)
         FROM reservations r5
         WHERE r5.customer_id = c.customer_id) AS total_guests,
        (SELECT MAX(p.payment_status)
         FROM payments p
         WHERE p.reservation_id = r.reservation_id) AS payment_status
    FROM customers c
    INNER JOIN reservations r 
        ON r.customer_id = c.customer_id
    -- Idinagdag itong filter logic
    WHERE (@fullname IS NULL OR c.full_name LIKE '%' + @fullname + '%')
    ORDER BY c.full_name, r.reservation_date DESC;
END; – CUSTOMER ACTIVTY

EXEC SP_getCustomerReservations


--STORED PROCEDURE 14 - aboveaveragemenu
CREATE PROCEDURE SP_aboveaveragemenu
AS
BEGIN 
	SELECT m.menu_id, m.food_name
FROM menu_items m
JOIN (
    SELECT menu_id, SUM(quantity) AS total_qty
    FROM order_items
    GROUP BY menu_id
) item_totals ON m.menu_id = item_totals.menu_id
WHERE item_totals.total_qty > (
    SELECT AVG(total_qty)
    FROM (
        SELECT SUM(quantity) AS total_qty
        FROM order_items
        GROUP BY menu_id
    ) totals_for_avg
)
AND m.menu_id IN (
    SELECT DISTINCT menu_id
    FROM order_items
);

END; — SALES REPORT
EXEC	SP_aboveaveragemenu


--STORED PROCEDURE 15 - SP_GrandTotalOverview
CREATE PROCEDURE SP_GrandDashboardStats
AS
BEGIN 
    SELECT 
        -- SUBQUERY 1: Kabuuang Pera (Lahat ng orders na may pera, pending man o hindi)
        (
            SELECT ISNULL(SUM(amount), 0)
            FROM payments
            WHERE payment_status IN ('Pending Verification', 'Ready', 'Delivered', 'Preparing', 'pending', 'Waiting for Verification','Confirmed')
        ) AS grand_total_sales,

        -- SUBQUERY 2: Kabuuang Bilang ng Orders
        (
            SELECT COUNT(*) FROM orders
        ) AS total_orders_count,

        -- SUBQUERY 3: Kabuuang Bilang ng Customers
        (
            SELECT COUNT(*) FROM customers
        ) AS total_customers_count;
END;
—DASHBOARD

SELECT DISTINCT payment_status FROM payments;

EXEC	SP_GrandTotalOverview


STORED PROCEDURE 16 - customerpaymentreport
CREATE PROCEDURE SP_customerpaymentreport
    @fullname NVARCHAR(100)
AS
BEGIN 
    SELECT
        c.full_name AS customer_name,
        SUM(p.amount) AS total_paid,
        o.total_amount AS order_total,
        CASE 
            WHEN SUM(p.amount) >= o.total_amount THEN 'Fully Paid'
            ELSE 'Initial / Partial Payment'
        END AS payment_status
    FROM payments p
    JOIN orders o ON p.order_id = o.order_id
    JOIN customers c ON o.customer_id = c.customer_id
    WHERE c.full_name = @fullname -- Exact Match, walang %
    GROUP BY c.full_name, o.total_amount;
END; —-CUSTOMER ACTIVITY

EXEC	SP_customerpaymentreport
		

-- 1. Add the Role column (default to normal Admin)
ALTER TABLE admins ADD role NVARCHAR(50) DEFAULT 'Admin';

-- 2. Make yourself the Super Admin (Assuming your admin_id is 1)
UPDATE admins SET role = 'Super Admin' WHERE admin_id = 2;

SELECT * FROM admins;

SELECT * FROM customers;

ALTER TABLE orders ADD payment_method NVARCHAR(50) NULL;

SELECT customer_id FROM orders WHERE order_id = 47;

CREATE TABLE PaymentSettings (
    id INT PRIMARY KEY IDENTITY(1,1),
    payment_name NVARCHAR(50), -- Halimbawa: 'GCash'
    qr_image_url NVARCHAR(MAX), -- Dito ise-save yung path ng picture
    is_active BIT DEFAULT 1
);

-- 1. Burahin ang lumang text column
ALTER TABLE PaymentSettings 
DROP COLUMN qr_image_url;

-- 2. Idagdag ang bagong image column
ALTER TABLE PaymentSettings  
ADD qr_image VARBINARY(MAX);

