

/* =========================================================
   CUSTOMERS
========================================================= */

CREATE OR ALTER PROCEDURE sp_Customers_Create
    @full_name NVARCHAR(255),
    @email NVARCHAR(255),
    @address NVARCHAR(MAX) = NULL,
    @password NVARCHAR(255),
    @is_verified BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO customers (full_name, email, address, [password], is_verified)
    VALUES (@full_name, @email, @address, @password, @is_verified);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS customer_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Customers_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM customers;
END;
GO

CREATE OR ALTER PROCEDURE sp_Customers_GetById
    @customer_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM customers WHERE customer_id = @customer_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Customers_Update
    @customer_id INT,
    @full_name NVARCHAR(255),
    @email NVARCHAR(255),
    @address NVARCHAR(MAX) = NULL,
    @password NVARCHAR(255),
    @is_verified BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE customers
    SET
        full_name = @full_name,
        email = @email,
        address = @address,
        [password] = @password,
        is_verified = @is_verified
    WHERE customer_id = @customer_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Customers_Delete
    @customer_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM customers WHERE customer_id = @customer_id;
END;
GO


/* =========================================================
   ADMINS
========================================================= */

CREATE OR ALTER PROCEDURE sp_Admins_Create
    @full_name NVARCHAR(255),
    @username NVARCHAR(255),
    @password NVARCHAR(255),
    @role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO admins (full_name, username, [password], role)
    VALUES (@full_name, @username, @password, @role);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS admin_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Admins_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM admins;
END;
GO

CREATE OR ALTER PROCEDURE sp_Admins_GetById
    @admin_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM admins WHERE admin_id = @admin_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Admins_Update
    @admin_id INT,
    @full_name NVARCHAR(255),
    @username NVARCHAR(255),
    @password NVARCHAR(255),
    @role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE admins
    SET
        full_name = @full_name,
        username = @username,
        [password] = @password,
        role = @role
    WHERE admin_id = @admin_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Admins_Delete
    @admin_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM admins WHERE admin_id = @admin_id;
END;
GO


/* =========================================================
   CATEGORIES
========================================================= */

CREATE OR ALTER PROCEDURE sp_Categories_Create
    @category_name NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO categories (category_name)
    VALUES (@category_name);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS category_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Categories_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM categories;
END;
GO

CREATE OR ALTER PROCEDURE sp_Categories_GetById
    @category_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM categories WHERE category_id = @category_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Categories_Update
    @category_id INT,
    @category_name NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE categories
    SET category_name = @category_name
    WHERE category_id = @category_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Categories_Delete
    @category_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM categories WHERE category_id = @category_id;
END;
GO


/* =========================================================
   MENU_ITEMS
========================================================= */

CREATE OR ALTER PROCEDURE sp_MenuItems_Create
    @category_id INT,
    @food_name NVARCHAR(255),
    @description NVARCHAR(MAX) = NULL,
    @price DECIMAL(18,2),
    @image NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO menu_items (category_id, food_name, description, price, image)
    VALUES (@category_id, @food_name, @description, @price, @image);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS menu_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_MenuItems_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM menu_items;
END;
GO

CREATE OR ALTER PROCEDURE sp_MenuItems_GetById
    @menu_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM menu_items WHERE menu_id = @menu_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_MenuItems_GetByCategory
    @category_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM menu_items WHERE category_id = @category_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_MenuItems_Update
    @menu_id INT,
    @category_id INT,
    @food_name NVARCHAR(255),
    @description NVARCHAR(MAX) = NULL,
    @price DECIMAL(18,2),
    @image NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE menu_items
    SET
        category_id = @category_id,
        food_name = @food_name,
        description = @description,
        price = @price,
        image = @image
    WHERE menu_id = @menu_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_MenuItems_Delete
    @menu_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM menu_items WHERE menu_id = @menu_id;
END;
GO


/* =========================================================
   DELIVERY_LOCATIONS
========================================================= */

CREATE OR ALTER PROCEDURE sp_DeliveryLocations_Create
    @location_name NVARCHAR(255),
    @delivery_fee DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO delivery_locations (location_name, delivery_fee)
    VALUES (@location_name, @delivery_fee);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS location_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeliveryLocations_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM delivery_locations;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeliveryLocations_GetById
    @location_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM delivery_locations WHERE location_id = @location_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeliveryLocations_Update
    @location_id INT,
    @location_name NVARCHAR(255),
    @delivery_fee DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE delivery_locations
    SET
        location_name = @location_name,
        delivery_fee = @delivery_fee
    WHERE location_id = @location_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeliveryLocations_Delete
    @location_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM delivery_locations WHERE location_id = @location_id;
END;
GO


/* =========================================================
   ORDERS
========================================================= */

CREATE OR ALTER PROCEDURE sp_Orders_Create
    @customer_id INT,
    @location_id INT = NULL,
    @confirmed_by_admin_id INT = NULL,
    @order_type NVARCHAR(50),
    @order_status NVARCHAR(50),
    @subtotal DECIMAL(18,2),
    @delivery_fee DECIMAL(18,2) = NULL,
    @total_amount DECIMAL(18,2),
    @downpayment_amount DECIMAL(18,2) = NULL,
    @remaining_balance DECIMAL(18,2) = NULL,
    @payment_status NVARCHAR(50),
    @order_date DATETIME,
    @preferred_time TIME = NULL,
    @delivery_address NVARCHAR(MAX) = NULL,
    @notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO orders
    (
        customer_id,
        location_id,
        confirmed_by_admin_id,
        order_type,
        order_status,
        subtotal,
        delivery_fee,
        total_amount,
        downpayment_amount,
        remaining_balance,
        payment_status,
        order_date,
        preferred_time,
        delivery_address,
        notes
    )
    VALUES
    (
        @customer_id,
        @location_id,
        @confirmed_by_admin_id,
        @order_type,
        @order_status,
        @subtotal,
        @delivery_fee,
        @total_amount,
        @downpayment_amount,
        @remaining_balance,
        @payment_status,
        @order_date,
        @preferred_time,
        @delivery_address,
        @notes
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS order_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Orders_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM orders;
END;
GO

CREATE OR ALTER PROCEDURE sp_Orders_GetById
    @order_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM orders WHERE order_id = @order_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Orders_GetByCustomer
    @customer_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM orders WHERE customer_id = @customer_id ORDER BY order_id DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Orders_Update
    @order_id INT,
    @customer_id INT,
    @location_id INT = NULL,
    @confirmed_by_admin_id INT = NULL,
    @order_type NVARCHAR(50),
    @order_status NVARCHAR(50),
    @subtotal DECIMAL(18,2),
    @delivery_fee DECIMAL(18,2) = NULL,
    @total_amount DECIMAL(18,2),
    @downpayment_amount DECIMAL(18,2) = NULL,
    @remaining_balance DECIMAL(18,2) = NULL,
    @payment_status NVARCHAR(50),
    @order_date DATETIME,
    @preferred_time TIME = NULL,
    @delivery_address NVARCHAR(MAX) = NULL,
    @notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE orders
    SET
        customer_id = @customer_id,
        location_id = @location_id,
        confirmed_by_admin_id = @confirmed_by_admin_id,
        order_type = @order_type,
        order_status = @order_status,
        subtotal = @subtotal,
        delivery_fee = @delivery_fee,
        total_amount = @total_amount,
        downpayment_amount = @downpayment_amount,
        remaining_balance = @remaining_balance,
        payment_status = @payment_status,
        order_date = @order_date,
        preferred_time = @preferred_time,
        delivery_address = @delivery_address,
        notes = @notes
    WHERE order_id = @order_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Orders_UpdateStatus
    @order_id INT,
    @order_status NVARCHAR(50),
    @confirmed_by_admin_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE orders
    SET
        order_status = @order_status,
        confirmed_by_admin_id = @confirmed_by_admin_id
    WHERE order_id = @order_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Orders_Delete
    @order_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM orders WHERE order_id = @order_id;
END;
GO


/* =========================================================
   ORDER_ITEMS
========================================================= */

CREATE OR ALTER PROCEDURE sp_OrderItems_Create
    @order_id INT,
    @menu_id INT,
    @quantity INT,
    @price DECIMAL(18,2),
    @subtotal DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO order_items (order_id, menu_id, quantity, price, subtotal)
    VALUES (@order_id, @menu_id, @quantity, @price, @subtotal);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS order_item_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_OrderItems_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM order_items;
END;
GO

CREATE OR ALTER PROCEDURE sp_OrderItems_GetById
    @order_item_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM order_items WHERE order_item_id = @order_item_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_OrderItems_GetByOrder
    @order_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM order_items WHERE order_id = @order_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_OrderItems_Update
    @order_item_id INT,
    @order_id INT,
    @menu_id INT,
    @quantity INT,
    @price DECIMAL(18,2),
    @subtotal DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE order_items
    SET
        order_id = @order_id,
        menu_id = @menu_id,
        quantity = @quantity,
        price = @price,
        subtotal = @subtotal
    WHERE order_item_id = @order_item_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_OrderItems_Delete
    @order_item_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM order_items WHERE order_item_id = @order_item_id;
END;
GO


/* =========================================================
   PAYMENTS
========================================================= */

CREATE OR ALTER PROCEDURE sp_Payments_Create
    @order_id INT = NULL,
    @reservation_id INT = NULL,
    @payment_method NVARCHAR(50),
    @amount DECIMAL(18,2),
    @reference_number NVARCHAR(255),
    @payment_status NVARCHAR(50),
    @payment_date DATETIME
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
        payment_date
    )
    VALUES
    (
        @order_id,
        @reservation_id,
        @payment_method,
        @amount,
        @reference_number,
        @payment_status,
        @payment_date
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS payment_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payments;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_GetById
    @payment_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payments WHERE payment_id = @payment_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_GetByOrder
    @order_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payments WHERE order_id = @order_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_GetByReservation
    @reservation_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payments WHERE reservation_id = @reservation_id;
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
    @payment_date DATETIME
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
        payment_date = @payment_date
    WHERE payment_id = @payment_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Payments_Delete
    @payment_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM payments WHERE payment_id = @payment_id;
END;
GO


/* =========================================================
   RESERVATIONS
========================================================= */

CREATE OR ALTER PROCEDURE sp_Reservations_Create
    @customer_id INT,
    @confirmed_by_admin_id INT = NULL,
    @reservation_date DATETIME,
    @reservation_time TIME,
    @number_of_guests INT,
    @reservation_status NVARCHAR(50),
    @notes NVARCHAR(MAX) = NULL,
    @downpayment_amount DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO reservations
    (
        customer_id,
        confirmed_by_admin_id,
        reservation_date,
        reservation_time,
        number_of_guests,
        reservation_status,
        notes,
        downpayment_amount
    )
    VALUES
    (
        @customer_id,
        @confirmed_by_admin_id,
        @reservation_date,
        @reservation_time,
        @number_of_guests,
        @reservation_status,
        @notes,
        @downpayment_amount
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS reservation_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Reservations_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM reservations;
END;
GO

CREATE OR ALTER PROCEDURE sp_Reservations_GetById
    @reservation_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM reservations WHERE reservation_id = @reservation_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Reservations_GetByCustomer
    @customer_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM reservations WHERE customer_id = @customer_id ORDER BY reservation_id DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Reservations_Update
    @reservation_id INT,
    @customer_id INT,
    @confirmed_by_admin_id INT = NULL,
    @reservation_date DATETIME,
    @reservation_time TIME,
    @number_of_guests INT,
    @reservation_status NVARCHAR(50),
    @notes NVARCHAR(MAX) = NULL,
    @downpayment_amount DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE reservations
    SET
        customer_id = @customer_id,
        confirmed_by_admin_id = @confirmed_by_admin_id,
        reservation_date = @reservation_date,
        reservation_time = @reservation_time,
        number_of_guests = @number_of_guests,
        reservation_status = @reservation_status,
        notes = @notes,
        downpayment_amount = @downpayment_amount
    WHERE reservation_id = @reservation_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_Reservations_Delete
    @reservation_id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM reservations WHERE reservation_id = @reservation_id;
END;
GO


/* =========================================================
   PAYMENT_QR_SETTINGS
========================================================= */

CREATE OR ALTER PROCEDURE sp_PaymentQr_Create
    @qr_name NVARCHAR(100),
    @qr_image VARBINARY(MAX),
    @is_active BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO payment_qr_settings (qr_name, qr_image, is_active)
    VALUES (@qr_name, @qr_image, @is_active);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payment_qr_settings;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetById
    @paymentqr_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payment_qr_settings
    WHERE paymentqr_id = @paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM payment_qr_settings
    WHERE is_active = 1;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_Update
    @paymentqr_id INT,
    @qr_name NVARCHAR(100),
    @qr_image VARBINARY(MAX),
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE payment_qr_settings
    SET
        qr_name = @qr_name,
        qr_image = @qr_image,
        is_active = @is_active
    WHERE paymentqr_id = @paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_SetActive
    @paymentqr_id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE payment_qr_settings
    SET is_active = 0;

    UPDATE payment_qr_settings
    SET is_active = 1
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

CREATE OR ALTER PROCEDURE sp_admins_CheckUsername
    @username NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM admins
    WHERE username = @username;
END;
GO
CREATE OR ALTER PROCEDURE sp_admins_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT admin_id, full_name, username, role
    FROM admins
    ORDER BY full_name ASC;
END;
GO

CREATE OR ALTER PROCEDURE sp_admins_CheckUsername
    @username NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM admins
    WHERE username = @username;
END;
GO

CREATE OR ALTER PROCEDURE sp_admins_Add
    @full_name NVARCHAR(255),
    @username NVARCHAR(255),
    @password NVARCHAR(255),
    @role NVARCHAR(50) = 'Admin'
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO admins (full_name, username, [password], role)
    VALUES (@full_name, @username, @password, @role);
END;
GO

CREATE OR ALTER PROCEDURE sp_admins_Delete
    @admin_id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM admins
    WHERE admin_id = @admin_id;
END;
GO


--nagoto

CREATE OR ALTER PROCEDURE sp_PaymentQr_Create
    @qr_name NVARCHAR(100),
    @qr_image VARBINARY(MAX),
    @is_active BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO payment_qr_settings (qr_name, qr_image, is_active, updated_at)
    VALUES (@qr_name, @qr_image, @is_active, GETDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS paymentqr_id;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM payment_qr_settings
    ORDER BY updated_at DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_PaymentQr_GetActive
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM payment_qr_settings
    WHERE is_active = 1
    ORDER BY updated_at DESC;
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

CREATE OR ALTER PROCEDURE sp_PaymentQr_SetActive
    @paymentqr_id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE payment_qr_settings
    SET is_active = 0;

    UPDATE payment_qr_settings
    SET is_active = 1,
        updated_at = GETDATE()
    WHERE paymentqr_id = @paymentqr_id;
END;
GO

--new
CREATE OR ALTER PROCEDURE SP_totalrevenvue
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(SUM(total_amount), 0) AS TotalSales,
        COUNT(*) AS TotalOrders,
        (
            SELECT COUNT(*)
            FROM reservations
        ) AS TotalReservations
    FROM orders;
END;
GO

-- orders
CREATE OR ALTER PROCEDURE sp_Orders_UpdateStatus
    @order_id INT,
    @order_status NVARCHAR(50),
    @confirmed_by_admin_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE orders
    SET
        order_status = @order_status,
        confirmed_by_admin_id = @confirmed_by_admin_id
    WHERE order_id = @order_id;
END;
GO

--orders

CREATE OR ALTER PROCEDURE sp_Orders_UpdateStatus
    @order_id INT,
    @order_status NVARCHAR(50),
    @confirmed_by_admin_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE orders
    SET
        order_status = @order_status,
        confirmed_by_admin_id = @confirmed_by_admin_id
    WHERE order_id = @order_id;
END;
GO