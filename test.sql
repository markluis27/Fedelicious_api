SELECT * FROM customers;

SELECT * FROM admins;

SELECT * FROM categories;
-- Babala: Mabubura lahat ng kaugnay na orders kapag ginawa mo ito
ALTER TABLE menu_items 
DROP CONSTRAINT FK_menu_items_categories; -- Palitan ang pangalan base sa actual constraint mo

ALTER TABLE menu_items 
ADD CONSTRAINT FK_menu_items_categories 
FOREIGN KEY (category_id) REFERENCES categories(category_id) 
ON DELETE CASCADE;
SELECT * FROM menu_items;
SELECT * FROM categories;
SELECT * FROM orders;
SELECT * FROM orders WHERE customer_id = '32';

SELECT * FROM reservations;

SELECT * FROM order_items;

SELECT * FROM payments;
DELETE FROM payments WHERE reference_number = '37';

SELECT order_id, customer_id, total_amount FROM Orders ORDER BY order_id DESC;

-- Tingnan mo kung anong mga order_id ang valid
SELECT order_id FROM orders;

SELECT p.payment_id, o.order_id, o.order_type, p.amount, p.payment_status 
FROM payments p
JOIN orders o ON p.order_id = o.order_id
WHERE o.order_id IN (11, 12);

DELETE FROM customers WHERE full_name = 'mac ortiz'; -- This example is from a Microsoft documentation example

ALTER TABLE payments
ADD customer_phone NVARCHAR(50) NULL;

ALTER TABLE payments ALTER COLUMN order_id INT NULL;
ALTER TABLE payments ALTER COLUMN reservation_id INT NULL;

ALTER TABLE orders
ALTER COLUMN location_id INT NULL;

-- Gawin na rin nating NULLABLE ang iba pang fields na pwedeng wala kapag Pick-up
ALTER TABLE orders ALTER COLUMN confirmed_by_admin_id INT NULL;
ALTER TABLE orders ALTER COLUMN delivery_fee DECIMAL(18,2) NULL;
ALTER TABLE orders ALTER COLUMN delivery_address NVARCHAR(MAX) NULL;


ALTER TABLE reservations ALTER COLUMN confirmed_by_admin_id INT NULL;
ALTER TABLE reservations ALTER COLUMN notes NVARCHAR(MAX) NULL;
ALTER TABLE reservations ALTER COLUMN downpayment_amount DECIMAL(18,2) NULL;

SELECT *FROM reservations;

SELECT * FROM orders;

SELECT * FROM customers;

SELECT * FROM reservations;

SELECT * FROM payments;

SELECT TOP 1 * FROM payments ORDER BY payment_id DESC;

DELETE FROM payments WHERE order_id IS NULL;

SELECT payment_id, reservation_id, reference_number, amount 
FROM payments 
WHERE payment_id = 14;


CREATE PROCEDURE SP_total_revenue
AS
BEGIN 
	SELECT SUM(amount) AS total_revenue 
FROM payments 
WHERE payment_status = 'Confirmed' OR payment_status = 'Success';
END;


CREATE PROCEDURE sp_GetReservationIntelligence
AS
BEGIN
    SELECT 
        r.reservation_id,
        r.customer_id,
        r.reservation_date,
        r.reservation_time,
        r.number_of_guests,
        r.reservation_status,
        p.reference_number,
        p.customer_phone,
        p.amount AS payment_amount
    FROM reservations r
    LEFT JOIN payments p ON r.reservation_id = p.reservation_id
    ORDER BY r.reservation_id DESC;
END;

ALTER TABLE menu_items 
ALTER COLUMN image NVARCHAR(MAX);


ALTER PROCEDURE sp_GetOrder
AS
BEGIN
    SELECT 
        o.order_id,
        o.order_date,                 -- BAGONG DAGDAG: Palitan mo ito kung iba ang pangalan (ex: created_at, date_ordered)
        o.order_status,
        o.order_type,
        
        -- BAGONG DAGDAG: Logic para sa Delivery Location
        CASE 
            WHEN o.order_type = 'Delivery' THEN c.address -- Palitan ang 'c.address' kung nasa Orders table yung location (ex: o.delivery_address)
            ELSE 'N/A' 
        END AS delivery_location,
        
        p.amount AS total_amount,     
        p.payment_method,             
        p.reference_number,
        p.customer_phone AS customerPhone, 
        o.notes,            
        c.full_name AS customerName
    FROM Orders o
    LEFT JOIN Customers c ON o.customer_id = c.customer_id
    LEFT JOIN Payments p ON o.order_id = p.order_id  
    ORDER BY o.order_id DESC;
END

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
END;


CREATE PROCEDURE SP_Listreservation
AS
BEGIN 
SELECT 
    c.full_name,
    r.reservation_date,
    r.reservation_time,
    MAX(r.number_of_guests) AS number_of_guests,
    r.reservation_status
FROM reservations r, customers c
WHERE r.customer_id = c.customer_id
GROUP BY 
    c.full_name,
    r.reservation_date,
    r.reservation_time,
    r.reservation_status
ORDER BY r.reservation_date DESC;

-- OKAY NA TO
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

--okay na to
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
END;

--okay na to 
CREATE PROCEDURE SP_Listreservation
AS
BEGIN 
SELECT 
    c.full_name,
    r.reservation_date,
    r.reservation_time,
    MAX(r.number_of_guests) AS number_of_guests,
    r.reservation_status
FROM reservations r, customers c
WHERE r.customer_id = c.customer_id
GROUP BY 
    c.full_name,
    r.reservation_date,
    r.reservation_time,
    r.reservation_status
ORDER BY r.reservation_date DESC;

