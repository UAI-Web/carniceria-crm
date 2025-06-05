
create table dbo.DigitoVerificadorV
(
	Tabla varchar(50) PRIMARY KEY,
	ValorDV varchar(2)
);

alter table dbo.Usuarios
add DigitoVerificadorH varchar(2);

