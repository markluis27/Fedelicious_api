SELECT * FROM customers;

SELECT * FROM categories;

SELECT * FROM orders;

SELECT * FROM order_items;

SELECT * FROM payments;

DELETE FROM customers WHERE full_name = 'mark luis ortiz'; -- This example is from a Microsoft documentation example

ALTER TABLE payments
ADD customer_phone NVARCHAR(50) NULL;
 