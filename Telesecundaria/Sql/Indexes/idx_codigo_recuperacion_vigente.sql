CREATE INDEX idx_codigo_recuperacion_vigente
    ON "CodigosRecuperacionTutor" ("claveTutorAspirante", usado, fecha_expiracion);
