DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'auth_catalog') THEN
        CREATE SCHEMA auth_catalog;
    END IF;
END $EF$;
CREATE TABLE IF NOT EXISTS auth_catalog.__ef_migrations_history (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM auth_catalog.__ef_migrations_history WHERE "migration_id" = '20260818193025_InitialCatalog') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'auth_catalog') THEN
            CREATE SCHEMA auth_catalog;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM auth_catalog.__ef_migrations_history WHERE "migration_id" = '20260818193025_InitialCatalog') THEN
    CREATE TABLE auth_catalog.tenant_user_index (
        normalized_email character varying(256) NOT NULL,
        tenant_id uuid NOT NULL,
        user_id uuid NOT NULL,
        created_at timestamp with time zone NOT NULL,
        CONSTRAINT pk_tenant_user_index PRIMARY KEY (normalized_email)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM auth_catalog.__ef_migrations_history WHERE "migration_id" = '20260818193025_InitialCatalog') THEN
    CREATE TABLE auth_catalog.tenants (
        id uuid NOT NULL,
        name character varying(200) NOT NULL,
        created_at timestamp with time zone NOT NULL,
        CONSTRAINT pk_tenants PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM auth_catalog.__ef_migrations_history WHERE "migration_id" = '20260818193025_InitialCatalog') THEN
    CREATE INDEX ix_tenant_user_index_tenant_id ON auth_catalog.tenant_user_index (tenant_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM auth_catalog.__ef_migrations_history WHERE "migration_id" = '20260818193025_InitialCatalog') THEN
    INSERT INTO auth_catalog.__ef_migrations_history (migration_id, product_version)
    VALUES ('20260818193025_InitialCatalog', '10.0.11');
    END IF;
END $EF$;
COMMIT;

