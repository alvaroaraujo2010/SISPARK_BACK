using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sispark_api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "celdas",
                columns: table => new
                {
                    id_celda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ubicacion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_tipo_vehiculo = table.Column<int>(type: "int", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_celdas", x => x.id_celda);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tipo_documento = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_documento = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre_completo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    correo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_usuario_creacion = table.Column<int>(type: "int", nullable: true),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    id_usuario_actualizacion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id_cliente);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "estados",
                columns: table => new
                {
                    id_estado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    modulo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados", x => x.id_estado);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_rol);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipos_servicio",
                columns: table => new
                {
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_servicio", x => x.id_tipo_servicio);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipos_vehiculo",
                columns: table => new
                {
                    id_tipo_vehiculo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_vehiculo", x => x.id_tipo_vehiculo);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apellido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ultimo_acceso = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_usuarios_estados_id_estado",
                        column: x => x.id_estado,
                        principalTable: "estados",
                        principalColumn: "id_estado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tarifas",
                columns: table => new
                {
                    id_tarifa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false),
                    id_tipo_vehiculo = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    fraccion_minutos = table.Column<int>(type: "int", nullable: true),
                    fecha_inicio_vigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_vigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    id_estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tarifas", x => x.id_tarifa);
                    table.ForeignKey(
                        name: "FK_tarifas_tipos_servicio_id_tipo_servicio",
                        column: x => x.id_tipo_servicio,
                        principalTable: "tipos_servicio",
                        principalColumn: "id_tipo_servicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tarifas_tipos_vehiculo_id_tipo_vehiculo",
                        column: x => x.id_tipo_vehiculo,
                        principalTable: "tipos_vehiculo",
                        principalColumn: "id_tipo_vehiculo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vehiculos",
                columns: table => new
                {
                    id_vehiculo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    placa = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    marca = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modelo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    color = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_tipo_vehiculo = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_usuario_creacion = table.Column<int>(type: "int", nullable: true),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    id_usuario_actualizacion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehiculos", x => x.id_vehiculo);
                    table.ForeignKey(
                        name: "FK_vehiculos_clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clientes",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehiculos_tipos_vehiculo_id_tipo_vehiculo",
                        column: x => x.id_tipo_vehiculo,
                        principalTable: "tipos_vehiculo",
                        principalColumn: "id_tipo_vehiculo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "mensualidades",
                columns: table => new
                {
                    id_mensualidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_vehiculo = table.Column<int>(type: "int", nullable: false),
                    id_tarifa = table.Column<int>(type: "int", nullable: false),
                    id_celda_fija = table.Column<int>(type: "int", nullable: true),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensualidades", x => x.id_mensualidad);
                    table.ForeignKey(
                        name: "FK_mensualidades_celdas_id_celda_fija",
                        column: x => x.id_celda_fija,
                        principalTable: "celdas",
                        principalColumn: "id_celda");
                    table.ForeignKey(
                        name: "FK_mensualidades_clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "clientes",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mensualidades_tarifas_id_tarifa",
                        column: x => x.id_tarifa,
                        principalTable: "tarifas",
                        principalColumn: "id_tarifa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mensualidades_vehiculos_id_vehiculo",
                        column: x => x.id_vehiculo,
                        principalTable: "vehiculos",
                        principalColumn: "id_vehiculo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "registros_parqueo",
                columns: table => new
                {
                    id_registro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_vehiculo = table.Column<int>(type: "int", nullable: false),
                    id_tipo_servicio = table.Column<int>(type: "int", nullable: false),
                    id_tarifa = table.Column<int>(type: "int", nullable: false),
                    id_celda = table.Column<int>(type: "int", nullable: true),
                    fecha_ingreso = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fecha_salida = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    minutos_consumidos = table.Column<int>(type: "int", nullable: true),
                    valor_calculado = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    valor_final = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    id_usuario_ingreso = table.Column<int>(type: "int", nullable: false),
                    id_usuario_salida = table.Column<int>(type: "int", nullable: true),
                    id_estado = table.Column<int>(type: "int", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_parqueo", x => x.id_registro);
                    table.ForeignKey(
                        name: "FK_registros_parqueo_celdas_id_celda",
                        column: x => x.id_celda,
                        principalTable: "celdas",
                        principalColumn: "id_celda");
                    table.ForeignKey(
                        name: "FK_registros_parqueo_tarifas_id_tarifa",
                        column: x => x.id_tarifa,
                        principalTable: "tarifas",
                        principalColumn: "id_tarifa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registros_parqueo_tipos_servicio_id_tipo_servicio",
                        column: x => x.id_tipo_servicio,
                        principalTable: "tipos_servicio",
                        principalColumn: "id_tipo_servicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registros_parqueo_usuarios_id_usuario_ingreso",
                        column: x => x.id_usuario_ingreso,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registros_parqueo_usuarios_id_usuario_salida",
                        column: x => x.id_usuario_salida,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario");
                    table.ForeignKey(
                        name: "FK_registros_parqueo_vehiculos_id_vehiculo",
                        column: x => x.id_vehiculo,
                        principalTable: "vehiculos",
                        principalColumn: "id_vehiculo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_mensualidades_id_celda_fija",
                table: "mensualidades",
                column: "id_celda_fija");

            migrationBuilder.CreateIndex(
                name: "IX_mensualidades_id_cliente",
                table: "mensualidades",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_mensualidades_id_tarifa",
                table: "mensualidades",
                column: "id_tarifa");

            migrationBuilder.CreateIndex(
                name: "IX_mensualidades_id_vehiculo",
                table: "mensualidades",
                column: "id_vehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_registros_parqueo_id_celda",
                table: "registros_parqueo",
                column: "id_celda");

            migrationBuilder.CreateIndex(
                name: "IX_registros_parqueo_id_tarifa",
                table: "registros_parqueo",
                column: "id_tarifa");

            migrationBuilder.CreateIndex(
                name: "IX_registros_parqueo_id_tipo_servicio",
                table: "registros_parqueo",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_registros_parqueo_id_usuario_ingreso",
                table: "registros_parqueo",
                column: "id_usuario_ingreso");

            migrationBuilder.CreateIndex(
                name: "IX_registros_parqueo_id_usuario_salida",
                table: "registros_parqueo",
                column: "id_usuario_salida");

            migrationBuilder.CreateIndex(
                name: "IX_registros_parqueo_id_vehiculo",
                table: "registros_parqueo",
                column: "id_vehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_tarifas_id_tipo_servicio",
                table: "tarifas",
                column: "id_tipo_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_tarifas_id_tipo_vehiculo",
                table: "tarifas",
                column: "id_tipo_vehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_estado",
                table: "usuarios",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_id_cliente",
                table: "vehiculos",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_vehiculos_id_tipo_vehiculo",
                table: "vehiculos",
                column: "id_tipo_vehiculo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mensualidades");

            migrationBuilder.DropTable(
                name: "registros_parqueo");

            migrationBuilder.DropTable(
                name: "celdas");

            migrationBuilder.DropTable(
                name: "tarifas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "vehiculos");

            migrationBuilder.DropTable(
                name: "tipos_servicio");

            migrationBuilder.DropTable(
                name: "estados");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "tipos_vehiculo");
        }
    }
}
