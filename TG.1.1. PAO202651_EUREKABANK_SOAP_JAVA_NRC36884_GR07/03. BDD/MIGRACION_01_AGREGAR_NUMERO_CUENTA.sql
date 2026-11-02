/* Ejecutar UNA sola vez sobre arquitectura_db antes de desplegar el nuevo WAR. */
USE arquitectura_db;

/* 1. Agrega el dato sin bloquear todavía los registros existentes. */
ALTER TABLE CUENTA ADD COLUMN CUENTA_NUMERO VARCHAR(10) NULL AFTER CUENTA_ID;

/* 2. Asigna a cada cuenta existente un número único, consecutivo y de 10 dígitos. */
SET @numero_cuenta := 1000000000;
UPDATE CUENTA
SET CUENTA_NUMERO = LPAD(@numero_cuenta := @numero_cuenta + 1, 10, '0')
WHERE CUENTA_NUMERO IS NULL
ORDER BY CUENTA_ID;

/* 3. Protege el contrato: número obligatorio y único. */
ALTER TABLE CUENTA
    MODIFY CUENTA_NUMERO VARCHAR(10) NOT NULL,
    ADD CONSTRAINT UK_CUENTA_NUMERO UNIQUE (CUENTA_NUMERO);

/* Verificación. */
SELECT CUENTA_ID, CUENTA_NUMERO, CUENTA_SALDO, CUENTA_ESTADO
FROM CUENTA
ORDER BY CUENTA_NUMERO;
