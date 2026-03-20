-- Categories (required for menu_items)
INSERT INTO categories (category_name) VALUES 
('Appetizers'),
('Main Course'),
('Desserts'),
('Drinks');

-- Menu items (6 sample items)
INSERT INTO menu_items (category_id, food_name, description, price, image) VALUES 
(1, 'Spring Rolls', 'Crispy vegetable rolls', 5.99, 'springrolls.jpg'),
(1, 'Calamari', 'Fried squid rings', 8.99, 'calamari.jpg'),
(2, 'Adobo Chicken', 'Filipino chicken adobo', 12.99, 'adobo.jpg'),
(2, 'Pancit Canton', 'Stir-fried noodles', 10.99, 'pancit.jpg'),
(3, 'Halo-Halo', 'Shaved ice dessert', 6.99, 'halohalo.jpg'),
(4, 'Iced Tea', 'Refreshing iced tea', 2.99, 'icetea.jpg');


-- Supporting tables: customers, delivery_locations, admins
INSERT INTO customers (full_name, email, address, password) VALUES 
('Juan Dela Cruz', 'juan@email.com', 'Santo Tomas, Batangas', 'pass123'),
('Maria Santos', 'maria@email.com', 'Tanauan, Batangas', 'pass456');

INSERT INTO delivery_locations (location_name, delivery_fee) VALUES 
('Santo Tomas', 50.00),
('Tanauan', 75.00);

INSERT INTO admins (full_name, username, password) VALUES 
('Admin User', 'admin1', 'adminpass');

-- Orders (3 sample orders)
INSERT INTO orders (customer_id, location_id, confirmed_by_admin_id, order_type, order_status, subtotal, delivery_fee, total_amount, downpayment_amount, remaining_balance, payment_status, order_date, preferred_time, delivery_address, notes) VALUES 
(1, 1, 1, 'Delivery', 'Confirmed', 25.97, 50.00, 75.97, 20.00, 55.97, 'Partial', '2026-02-01', '18:00:00', 'Santo Tomas Center', 'Extra spicy'),
(1, 1, 1, 'Pickup', 'Delivered', 19.98, NULL, 19.98, NULL, NULL, 'Paid', '2026-02-10', NULL, NULL, NULL),
(2, 2, 1, 'Delivery', 'Confirmed', 36.96, 75.00, 111.96, 50.00, 61.96, 'Partial', '2026-02-15', '19:00:00', 'Tanauan Blvd', 'No onions');


-- Order items (links orders to menu; enables aggregate)
INSERT INTO order_items (order_id, menu_id, quantity, price, subtotal) VALUES 
(1, 1, 2, 5.99, 11.98),  -- Spring Rolls x2
(1, 3, 1, 12.99, 12.99), -- Adobo x1
(1, 6, 1, 2.99, 2.99),   -- Iced Tea x1
(2, 2, 1, 8.99, 8.99),   -- Calamari x1
(2, 4, 1, 10.99, 10.99), -- Pancit x1
(3, 3, 2, 12.99, 25.98), -- Adobo x2
(3, 5, 1, 6.99, 6.99),   -- Halo-Halo x1
(3, 6, 1, 2.99, 2.99);   -- Iced Tea x1


INSERT INTO reservations (customer_id, confirmed_by_admin_id, reservation_date, reservation_time, number_of_guests, reservation_status, notes, downpayment_amount) VALUES 
(1, 1, '2026-02-20', '19:00:00', 4, 'Confirmed', 'Birthday celebration', 500.00),
(2, 1, '2026-02-25', '18:30:00', 2, 'Pending', 'Anniversary dinner', NULL),
(1, NULL, '2026-03-01', '20:00:00', 6, 'Pending', 'Family reunion', NULL);


INSERT INTO payments (order_id, reservation_id, payment_method, amount, reference_number, payment_status, payment_date) VALUES 
-- Order payments (matching your 3 orders)
(1, NULL, 'GCash', 20.00, 'GC123456', 'Paid', '2026-02-01'),  -- Juan's downpayment
(2, NULL, 'Cash', 19.98, 'CASH002', 'Paid', '2026-02-10'),     -- Juan's pickup full payment


-- Reservation payments
(NULL, 1, 'GCash', 500.00, 'GC789012', 'Paid', '2026-02-18');  -- Reservation downpayment
