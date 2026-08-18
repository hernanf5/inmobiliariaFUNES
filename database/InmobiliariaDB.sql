DROP DATABASE IF EXISTS InmobiliariaDB;
CREATE DATABASE InmobiliariaDB
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE InmobiliariaDB;

-- Tabla: Usuario
CREATE TABLE Usuario (
    IdUsuario       INT AUTO_INCREMENT PRIMARY KEY,
    Email           VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash    VARCHAR(256) NOT NULL,
    Nombre          VARCHAR(100) NOT NULL,
    Rol             VARCHAR(20)  NOT NULL,
    AvatarUrl       VARCHAR(300) NULL,
    Activo          TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT CK_Usuario_Rol CHECK (Rol IN ('Administrador', 'Empleado'))
) ENGINE=InnoDB;

-- Tabla: Propietario
CREATE TABLE Propietario (
    IdPropietario   INT AUTO_INCREMENT PRIMARY KEY,
    Nombre          VARCHAR(100) NOT NULL,
    Apellido        VARCHAR(100) NOT NULL,
    DniCuit         VARCHAR(20)  NOT NULL UNIQUE,
    Telefono        VARCHAR(30)  NULL,
    Email           VARCHAR(150) NOT NULL,
    Activo          TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- Tabla: Inquilino
CREATE TABLE Inquilino (
    IdInquilino     INT AUTO_INCREMENT PRIMARY KEY,
    Dni             VARCHAR(20)  NOT NULL UNIQUE,
    Nombre          VARCHAR(100) NOT NULL,
    Apellido        VARCHAR(100) NOT NULL,
    Telefono        VARCHAR(30)  NULL,
    Email           VARCHAR(150) NOT NULL,
    Activo          TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- Tabla: TipoInmueble (ABM)
CREATE TABLE TipoInmueble (
    IdTipoInmueble  INT AUTO_INCREMENT PRIMARY KEY,
    Nombre          VARCHAR(50) NOT NULL UNIQUE,
    Activo          TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB;

-- Tabla: Inmueble
CREATE TABLE Inmueble (
    IdInmueble          INT AUTO_INCREMENT PRIMARY KEY,
    IdPropietario       INT NOT NULL,
    IdTipoInmueble      INT NOT NULL,
    Direccion           VARCHAR(200) NOT NULL,
    Cupo                INT NOT NULL,
    Latitud             DECIMAL(9,6) NULL,
    Longitud            DECIMAL(9,6) NULL,
    PrecioPorDia        DECIMAL(12,2) NOT NULL,
    PorcentajeReserva   DECIMAL(5,2) NOT NULL DEFAULT 30.00,
    Estado              VARCHAR(20) NOT NULL DEFAULT 'Disponible',
    CONSTRAINT CK_Inmueble_Estado CHECK (Estado IN ('Disponible', 'Suspendido')),
    CONSTRAINT FK_Inmueble_Propietario FOREIGN KEY (IdPropietario) REFERENCES Propietario(IdPropietario),
    CONSTRAINT FK_Inmueble_TipoInmueble FOREIGN KEY (IdTipoInmueble) REFERENCES TipoInmueble(IdTipoInmueble)
) ENGINE=InnoDB;

-- Tabla: ImagenInmueble
CREATE TABLE ImagenInmueble (
    IdImagen        INT AUTO_INCREMENT PRIMARY KEY,
    IdInmueble      INT NOT NULL,
    Url             VARCHAR(300) NOT NULL,
    EsPortada       TINYINT(1) NOT NULL DEFAULT 0,
    CONSTRAINT FK_ImagenInmueble_Inmueble FOREIGN KEY (IdInmueble) REFERENCES Inmueble(IdInmueble)
) ENGINE=InnoDB;

-- Tabla: Reserva
CREATE TABLE Reserva (
    IdReserva               INT AUTO_INCREMENT PRIMARY KEY,
    IdInquilino              INT NOT NULL,
    IdInmueble                INT NOT NULL,
    IdReservaOrigen            INT NULL,
    MontoPorDia              DECIMAL(12,2) NOT NULL,
    FechaDesde               DATE NOT NULL,
    FechaHastaOriginal        DATE NOT NULL,
    FechaTerminacion          DATE NULL,
    Multa                    DECIMAL(12,2) NULL,
    Estado                   VARCHAR(30) NOT NULL DEFAULT 'Vigente',
    IdUsuarioCreador          INT NOT NULL,
    IdUsuarioTerminador       INT NULL,
    CONSTRAINT CK_Reserva_Estado CHECK (Estado IN ('Vigente', 'Finalizada', 'Terminada anticipadamente')),
    CONSTRAINT CK_Reserva_Fechas CHECK (FechaHastaOriginal > FechaDesde),
    CONSTRAINT FK_Reserva_Inquilino FOREIGN KEY (IdInquilino) REFERENCES Inquilino(IdInquilino),
    CONSTRAINT FK_Reserva_Inmueble FOREIGN KEY (IdInmueble) REFERENCES Inmueble(IdInmueble),
    CONSTRAINT FK_Reserva_ReservaOrigen FOREIGN KEY (IdReservaOrigen) REFERENCES Reserva(IdReserva),
    CONSTRAINT FK_Reserva_UsuarioCreador FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuario(IdUsuario),
    CONSTRAINT FK_Reserva_UsuarioTerminador FOREIGN KEY (IdUsuarioTerminador) REFERENCES Usuario(IdUsuario)
) ENGINE=InnoDB;

-- Tabla: Pago
CREATE TABLE Pago (
    IdPago               INT AUTO_INCREMENT PRIMARY KEY,
    IdReserva            INT NOT NULL,
    Concepto             VARCHAR(150) NOT NULL,
    FechaPago            DATE NOT NULL,
    Importe              DECIMAL(12,2) NOT NULL,
    Estado               VARCHAR(20) NOT NULL DEFAULT 'Activo',
    IdUsuarioCreador     INT NOT NULL,
    IdUsuarioAnulador    INT NULL,
    CONSTRAINT CK_Pago_Estado CHECK (Estado IN ('Activo', 'Anulado')),
    CONSTRAINT FK_Pago_Reserva FOREIGN KEY (IdReserva) REFERENCES Reserva(IdReserva),
    CONSTRAINT FK_Pago_UsuarioCreador FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuario(IdUsuario),
    CONSTRAINT FK_Pago_UsuarioAnulador FOREIGN KEY (IdUsuarioAnulador) REFERENCES Usuario(IdUsuario)
) ENGINE=InnoDB;
