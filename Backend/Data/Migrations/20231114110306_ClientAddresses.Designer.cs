﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(UnitOfWork))]
    [Migration("20231114110306_ClientAddresses")]
    partial class ClientAddresses
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Models.Entities.AddressType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("AddressTypes");
                });

            modelBuilder.Entity("Models.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.HasKey("Id");

                    b.ToTable("Clients");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Client");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Models.Entities.ClientAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<string>("HouseNumber")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.Property<string>("Street")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("StreetNumber")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<int>("TypeObjectId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("TypeObjectId");

                    b.ToTable("ClientAddresses");
                });

            modelBuilder.Entity("Models.Entities.Parameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("integer");

                    b.Property<bool>("Required")
                        .HasColumnType("boolean");

                    b.Property<int?>("SubProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("SubProductId");

                    b.HasIndex("Type");

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("Models.Entities.ParameterOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ParameterId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("ParameterId");

                    b.ToTable("ParameterOptions");
                });

            modelBuilder.Entity("Models.Entities.ParameterType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("ParameterTypes");
                });

            modelBuilder.Entity("Models.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AnonymousSaleAllowed")
                        .HasColumnType("boolean");

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("money");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("Status");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Models.Entities.ProductStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.HasKey("Id");

                    b.ToTable("ProductStatuses");
                });

            modelBuilder.Entity("Models.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Models.Entities.Sale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<decimal>("FinalPrice")
                        .HasColumnType("money");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("SellerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SellerId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("Models.Entities.SaleParameter", b =>
                {
                    b.Property<int>("SaleId")
                        .HasColumnType("integer");

                    b.Property<int>("ParameterId")
                        .HasColumnType("integer");

                    b.Property<int?>("OptionId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("SaleId", "ParameterId");

                    b.HasIndex("OptionId");

                    b.HasIndex("ParameterId");

                    b.ToTable("SaleParameters");
                });

            modelBuilder.Entity("Models.Entities.SubProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("money");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("SubProducts");
                });

            modelBuilder.Entity("Models.Entities.SubProductInProduct", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("SubProductId")
                        .HasColumnType("integer");

                    b.Property<decimal>("InProductPrice")
                        .HasColumnType("money");

                    b.HasKey("ProductId", "SubProductId");

                    b.HasIndex("SubProductId");

                    b.ToTable("SubProductsInProducts");
                });

            modelBuilder.Entity("Models.Entities.SubProductInSale", b =>
                {
                    b.Property<int>("SaleId")
                        .HasColumnType("integer");

                    b.Property<int>("SubProductId")
                        .HasColumnType("integer");

                    b.Property<decimal>("InSalePrice")
                        .HasColumnType("money");

                    b.Property<bool>("ManualOverride")
                        .HasColumnType("boolean");

                    b.HasKey("SaleId", "SubProductId");

                    b.HasIndex("SubProductId");

                    b.ToTable("SubProductsInSales");
                });

            modelBuilder.Entity("Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("Hash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("bytea");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserRoles", b =>
                {
                    b.Property<int>("RolesId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("RolesId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Models.Entities.FirmClient", b =>
                {
                    b.HasBaseType("Models.Entities.Client");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Nip")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Regon")
                        .HasMaxLength(14)
                        .HasColumnType("character varying(14)");

                    b.HasDiscriminator().HasValue("FirmClient");
                });

            modelBuilder.Entity("Models.Entities.PersonClient", b =>
                {
                    b.HasBaseType("Models.Entities.Client");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Pesel")
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.HasDiscriminator().HasValue("PersonClient");
                });

            modelBuilder.Entity("Models.Entities.ClientAddress", b =>
                {
                    b.HasOne("Models.Entities.Client", "Client")
                        .WithMany("Addresses")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.AddressType", "TypeObject")
                        .WithMany()
                        .HasForeignKey("TypeObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("TypeObject");
                });

            modelBuilder.Entity("Models.Entities.Parameter", b =>
                {
                    b.HasOne("Models.Entities.Product", "Product")
                        .WithMany("Parameters")
                        .HasForeignKey("ProductId");

                    b.HasOne("Models.Entities.SubProduct", "SubProduct")
                        .WithMany("Parameters")
                        .HasForeignKey("SubProductId");

                    b.HasOne("Models.Entities.ParameterType", "TypeObject")
                        .WithMany()
                        .HasForeignKey("Type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("SubProduct");

                    b.Navigation("TypeObject");
                });

            modelBuilder.Entity("Models.Entities.ParameterOption", b =>
                {
                    b.HasOne("Models.Entities.Parameter", "Parameter")
                        .WithMany("Options")
                        .HasForeignKey("ParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parameter");
                });

            modelBuilder.Entity("Models.Entities.Product", b =>
                {
                    b.HasOne("Models.Entities.ProductStatus", "StatusObject")
                        .WithMany()
                        .HasForeignKey("Status")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StatusObject");
                });

            modelBuilder.Entity("Models.Entities.Sale", b =>
                {
                    b.HasOne("Models.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.Product", "Product")
                        .WithMany("Sales")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.User", "Seller")
                        .WithMany()
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Product");

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("Models.Entities.SaleParameter", b =>
                {
                    b.HasOne("Models.Entities.ParameterOption", "Option")
                        .WithMany()
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Models.Entities.Parameter", "Parameter")
                        .WithMany("SaleParameters")
                        .HasForeignKey("ParameterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.Sale", "Sale")
                        .WithMany("SaleParameters")
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Option");

                    b.Navigation("Parameter");

                    b.Navigation("Sale");
                });

            modelBuilder.Entity("Models.Entities.SubProductInProduct", b =>
                {
                    b.HasOne("Models.Entities.Product", "Product")
                        .WithMany("SubProductInProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.SubProduct", "SubProduct")
                        .WithMany("SubProductInProducts")
                        .HasForeignKey("SubProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("SubProduct");
                });

            modelBuilder.Entity("Models.Entities.SubProductInSale", b =>
                {
                    b.HasOne("Models.Entities.Sale", "Sale")
                        .WithMany("SubProducts")
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.SubProduct", "SubProduct")
                        .WithMany("SubProductInSales")
                        .HasForeignKey("SubProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sale");

                    b.Navigation("SubProduct");
                });

            modelBuilder.Entity("UserRoles", b =>
                {
                    b.HasOne("Models.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Models.Entities.Client", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("Models.Entities.Parameter", b =>
                {
                    b.Navigation("Options");

                    b.Navigation("SaleParameters");
                });

            modelBuilder.Entity("Models.Entities.Product", b =>
                {
                    b.Navigation("Parameters");

                    b.Navigation("Sales");

                    b.Navigation("SubProductInProducts");
                });

            modelBuilder.Entity("Models.Entities.Sale", b =>
                {
                    b.Navigation("SaleParameters");

                    b.Navigation("SubProducts");
                });

            modelBuilder.Entity("Models.Entities.SubProduct", b =>
                {
                    b.Navigation("Parameters");

                    b.Navigation("SubProductInProducts");

                    b.Navigation("SubProductInSales");
                });
#pragma warning restore 612, 618
        }
    }
}
