CREATE INDEX IF NOT EXISTS idx_codigo_recuperacion_vigente
    ON "CodigosRecuperacionTutor" ("claveTutorAspirante", usado, fecha_expiracion);
