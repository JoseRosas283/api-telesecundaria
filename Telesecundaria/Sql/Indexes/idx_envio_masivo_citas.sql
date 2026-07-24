CREATE INDEX idx_envio_masivo_citas ON "RevisionesAceptadas" ("claveConvocatoria", activo) 
WHERE activo = TRUE;