

CREATE TABLE customers (
    customer_id INT PRIMARY KEY IDENTITY(1,1),
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    address VARCHAR(MAX),
    password VARCHAR(255) NOT NULL
);

CREATE TABLE admins (
    admin_id INT PRIMARY KEY IDENTITY(1,1),
    full_name VARCHAR(255) NOT NULL,
    username VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);

CREATE TABLE categories (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE menu_items (
    menu_id INT PRIMARY KEY IDENTITY(1,1),
    category_id INT NULL,
    food_name VARCHAR(255) NOT NULL,
    description VARCHAR(MAX),
    price DECIMAL(10,2) NOT NULL,
    image VARCHAR(255),
    CONSTRAINT FK_menu_category
        FOREIGN KEY (category_id)
        REFERENCES categories(category_id)
        ON DELETE SET NULL
);

CREATE TABLE delivery_locations (
    location_id INT PRIMARY KEY IDENTITY(1,1),
    location_name VARCHAR(255) NOT NULL,
    delivery_fee DECIMAL(10,2) NOT NULL
);

CREATE TABLE orders (
    order_id INT PRIMARY KEY IDENTITY(1,1),
    customer_id INT NOT NULL,
    location_id INT NOT NULL,
    confirmed_by_admin_id INT NULL,
    order_type VARCHAR(50),
    order_status VARCHAR(50),
    subtotal DECIMAL(10,2),
    delivery_fee DECIMAL(10,2),
    total_amount DECIMAL(10,2),
    downpayment_amount DECIMAL(10,2),
    remaining_balance DECIMAL(10,2),
    payment_status VARCHAR(50),
    order_date DATE,
    preferred_time TIME,
    delivery_address VARCHAR(MAX),
    notes VARCHAR(MAX),
    CONSTRAINT FK_orders_customer
        FOREIGN KEY (customer_id)
        REFERENCES customers(customer_id)
        ON DELETE CASCADE,
    CONSTRAINT FK_orders_location
        FOREIGN KEY (location_id)
        REFERENCES delivery_locations(location_id),
    CONSTRAINT FK_orders_admin
        FOREIGN KEY (confirmed_by_admin_id)
        REFERENCES admins(admin_id)
        ON DELETE SET NULL
);

CREATE TABLE order_items (
    order_item_id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL,
    menu_id INT NOT NULL,
    quantity INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_orderitems_order
        FOREIGN KEY (order_id)
        REFERENCES orders(order_id)
        ON DELETE CASCADE,
    CONSTRAINT FK_orderitems_menu
        FOREIGN KEY (menu_id)
        REFERENCES menu_items(menu_id)
);

CREATE TABLE reservations (
    reservation_id INT PRIMARY KEY IDENTITY(1,1),
    customer_id INT NOT NULL,
    confirmed_by_admin_id INT NULL,
    reservation_date DATE,
    reservation_time TIME,
    number_of_guests INT,
    reservation_status VARCHAR(50),
    notes VARCHAR(MAX),
    downpayment_amount DECIMAL(10,2),
    CONSTRAINT FK_reservations_customer
        FOREIGN KEY (customer_id)
        REFERENCES customers(customer_id)
        ON DELETE CASCADE,
    CONSTRAINT FK_reservations_admin
        FOREIGN KEY (confirmed_by_admin_id)
        REFERENCES admins(admin_id)
        ON DELETE SET NULL
);

CREATE TABLE payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NULL,
    reservation_id INT NULL,
    payment_method VARCHAR(50),
    amount DECIMAL(10,2),
    reference_number VARCHAR(255) UNIQUE,
    payment_status VARCHAR(50),
    payment_date DATE,
    CONSTRAINT FK_payments_order
        FOREIGN KEY (order_id)
        REFERENCES orders(order_id),
    CONSTRAINT FK_payments_reservation
        FOREIGN KEY (reservation_id)
        REFERENCES reservations(reservation_id)
);
