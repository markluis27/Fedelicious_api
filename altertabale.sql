SELECT * FROM admins;
SELECT * FROM orders
SELECT * FROM payments;
SELECT * FROM payment_qr_settings;

EXEC sp_GetOrder

ALTER TABLE payment_qr_settings
ADD qr_accname NVARCHAR(150) NULL;
GO

ALTER TABLE payments
DROP CONSTRAINT FK_payments_order;
GO

ALTER TABLE payments
ADD CONSTRAINT FK_payments_order
FOREIGN KEY (order_id) REFERENCES orders(order_id)
ON DELETE CASCADE;
GO

ALTER TABLE payments
ADD CONSTRAINT FK_payments_payment_qr_settings
FOREIGN KEY (paymentqr_id)
REFERENCES payment_qr_settings(paymentqr_id)
ON DELETE CASCADE;
GO

-- mag add ng column parasa payemnts
ALTER TABLE payments
ADD paymentqr_id INT NULL;
GO

ALTER TABLE payments
ADD CONSTRAINT FK_payments_paymentqr
FOREIGN KEY (paymentqr_id)
REFERENCES payment_qr_settings(paymentqr_id);
GO

--paymentsqrsettings

CREATE OR ALTER PROCEDURE sp_PaymentQr_Create
    @qr_name NVARCHAR(100),
    @qr_accname NVARCHAR(150),
    @qr_image VARBINARY(MAX),
    @is_active BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO payment_qr_settings (qr_name, qr_accname, qr_image, is_active, updated_at)
    VALUES (@qr_name, @qr_accname, @qr_image, @is_active, GETDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT paymentqr_id, qr_name, qr_accname, qr_image, is_active, updated_at
    FROM payment_qr_settings
    ORDER BY updated_at DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetActive
AS
BEGIN
    SET NOCOUNT ON;

    SELECT paymentqr_id, qr_name, qr_accname, qr_image, is_active, updated_at
    FROM payment_qr_settings
    WHERE is_active = 1
    ORDER BY updated_at DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetById
    @paymentqr_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT paymentqr_id, qr_name, qr_accname, qr_image, is_active, updated_at
    FROM payment_qr_settings
    WHERE paymentqr_id = @paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_Delete
    @paymentqr_id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM payment_qr_settings
    WHERE paymentqr_id = @paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_Update
    @paymentqr_id INT,
    @qr_name NVARCHAR(100),
    @qr_accname NVARCHAR(150),
    @qr_image VARBINARY(MAX),
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE payment_qr_settings
    SET
        qr_name = @qr_name,
        qr_accname = @qr_accname,
        qr_image = @qr_image,
        is_active = @is_active,
        updated_at = GETDATE()
    WHERE paymentqr_id = @paymentqr_id;
END;
GO


--payments
CREATE OR ALTER PROCEDURE sp_Payments_Create
    @order_id INT = NULL,
    @reservation_id INT = NULL,
    @payment_method NVARCHAR(50),
    @amount DECIMAL(18,2),
    @reference_number NVARCHAR(255),
    @payment_status NVARCHAR(50),
    @payment_date DATETIME,
    @paymentqr_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO payments
    (
        order_id,
        reservation_id,
        payment_method,
        amount,
        reference_number,
        payment_status,
        payment_date,
        paymentqr_id
    )
    VALUES
    (
        @order_id,
        @reservation_id,
        @payment_method,
        @amount,
        @reference_number,
        @payment_status,
        @payment_date,
        @paymentqr_id
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS payment_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_Update
    @payment_id INT,
    @order_id INT = NULL,
    @reservation_id INT = NULL,
    @payment_method NVARCHAR(50),
    @amount DECIMAL(18,2),
    @reference_number NVARCHAR(255),
    @payment_status NVARCHAR(50),
    @payment_date DATETIME,
    @paymentqr_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE payments
    SET
        order_id = @order_id,
        reservation_id = @reservation_id,
        payment_method = @payment_method,
        amount = @amount,
        reference_number = @reference_number,
        payment_status = @payment_status,
        payment_date = @payment_date,
        paymentqr_id = @paymentqr_id
    WHERE payment_id = @payment_id;
END;
GO

ALTER PROCEDURE [dbo].[sp_GetReservationIntelligence]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        r.reservation_id,
        r.customer_id,
        r.reservation_date,
        r.reservation_time,
        r.number_of_guests,
        r.reservation_status,
        r.notes,

        p.reference_number,
        p.amount AS payment_amount,
        p.paymentqr_id,

        q.qr_name,
        q.qr_accname   -- ✅ IDINAGDAG

    FROM reservations r

    LEFT JOIN payments p 
        ON r.reservation_id = p.reservation_id

    LEFT JOIN payment_qr_settings q
        ON p.paymentqr_id = q.paymentqr_id

    ORDER BY r.reservation_id DESC;
END
GO

ALTER PROCEDURE [dbo].[sp_GetOrder]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        o.order_id,
        o.order_date,
        o.order_status,
        o.order_type,

        CASE 
            WHEN o.order_type = 'Delivery' THEN c.address
            ELSE 'N/A'
        END AS delivery_location,

        p.amount AS total_amount,
        p.payment_method,
        p.reference_number,
        p.paymentqr_id,

        q.qr_name,
        q.qr_accname,   -- ✅ IDINAGDAG

        o.notes,
        c.full_name AS customerName

    FROM orders o

    LEFT JOIN customers c 
        ON o.customer_id = c.customer_id

    LEFT JOIN payments p 
        ON o.order_id = p.order_id

    LEFT JOIN payment_qr_settings q
        ON p.paymentqr_id = q.paymentqr_id

    ORDER BY o.order_id DESC;
END
GO

SELECT * 
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'payment_qr_settings'
AND COLUMN_NAME = 'qr_accname';



---BAGOTO TALAGA SA PAYMENNTS  UPDATED

CREATE OR ALTER PROCEDURE sp_Payments_CheckReference
    @reference_number NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM payments
    WHERE reference_number = @reference_number;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_Create
    @order_id INT = NULL,
    @reservation_id INT = NULL,
    @payment_method NVARCHAR(50),
    @amount DECIMAL(18,2),
    @reference_number NVARCHAR(255),
    @customer_phone NVARCHAR(50) = NULL,
    @payment_status NVARCHAR(50),
    @payment_date DATETIME,
    @paymentqr_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO payments
    (
        order_id,
        reservation_id,
        payment_method,
        amount,
        reference_number,
        customer_phone,
        payment_status,
        payment_date,
        paymentqr_id
    )
    VALUES
    (
        @order_id,
        @reservation_id,
        @payment_method,
        @amount,
        @reference_number,
        @customer_phone,
        @payment_status,
        @payment_date,
        @paymentqr_id
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS payment_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_Update
    @payment_id INT,
    @order_id INT = NULL,
    @reservation_id INT = NULL,
    @payment_method NVARCHAR(50),
    @amount DECIMAL(18,2),
    @reference_number NVARCHAR(255),
    @customer_phone NVARCHAR(50) = NULL,
    @payment_status NVARCHAR(50),
    @payment_date DATETIME,
    @paymentqr_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE payments
    SET
        order_id = @order_id,
        reservation_id = @reservation_id,
        payment_method = @payment_method,
        amount = @amount,
        reference_number = @reference_number,
        customer_phone = @customer_phone,
        payment_status = @payment_status,
        payment_date = @payment_date,
        paymentqr_id = @paymentqr_id
    WHERE payment_id = @payment_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_GetByOrder
    @order_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM payments
    WHERE order_id = @order_id
    ORDER BY payment_id DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_GetByReservation
    @reservation_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM payments
    WHERE reservation_id = @reservation_id
    ORDER BY payment_id DESC;
END;
GO