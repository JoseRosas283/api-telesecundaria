CREATE INDEX IF NOT EXISTS idx_envio_masivo_citas 
    ON "RevisionesAceptadas" ("claveConvocatoria", "Estado") 
    WHERE "Estado" = TRUE;