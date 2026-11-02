-- ============================================================
-- CLIENTES (mismos CEDULA y NOMBRE que en BANQUITO)
-- ============================================================
INSERT INTO CLIENTE (COD_CLIENTE, CEDULA, NOMBRE) VALUES
(106, '0102030405', 'Ana Torres'),
(107, '0203040506', 'Luis Pérez'),
(108, '0304050607', 'María Gómez'),
(109, '0405060708', 'Carlos Ruiz'),
(110, '0506070809', 'Sofía Mendieta');

-- ============================================================
-- ITEM_INVENTARIO (catálogo de productos)
-- ============================================================
INSERT INTO ITEM_INVENTARIO (NUM_INVENTARIO, NOMBRE, PRECIO) VALUES
(1, 'Samsung Galaxy A15',      600.00),
(2, 'Xiaomi Redmi Note 13',    750.00),
(3, 'Motorola Moto G24',       550.00),
(4, 'iPhone 13 128GB',        2500.00),  
(5, 'Samsung Galaxy S24',      900.00);  

-- ============================================================
-- FACTURAS (ligadas a los mismos clientes)
-- ============================================================
INSERT INTO FACTURA (NUM_FACTURA, COD_CLIENTE, FECHA, TIPO_PAGO, VALOR) VALUES
(5001, 106, '2026-10-28', 'EFECTIVO', 348.00),  -- Ana: 600 - 42%
(5005, 110, '2026-11-01', 'CREDITO',  900.00);  -- Sofia: aprobado por el banco

-- ============================================================
-- ITEM_FACTURA (detalle; ligado a FACTURA e ITEM_INVENTARIO)
-- ============================================================
INSERT INTO ITEM_FACTURA (NUM_ITEM, NUM_FACTURA, NUM_INVENTARIO, VALOR, CANTIDAD) VALUES
(1, 5001, 1, 600.00, 1),  
(2, 5005, 5, 900.00, 1);  
