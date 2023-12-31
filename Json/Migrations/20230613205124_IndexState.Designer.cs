﻿// <auto-generated />
using System;
using Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Json.Migrations
{
    [DbContext(typeof(SqlServerDemoContext))]
    [Migration("20230613205124_IndexState")]
    partial class IndexState
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Json.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AddressDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("State")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("nvarchar(450)")
                        .HasComputedColumnSql("JSON_VALUE([BillingAddress], '$.State')");

                    b.HasKey("Id");

                    b.HasIndex("State");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Json.Employee", b =>
                {
                    b.OwnsOne("Json.AddressDetails", "BillingAddress", b1 =>
                        {
                            b1.Property<int>("EmployeeId")
                                .HasColumnType("int");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employees");

                            b1.ToJson("BillingAddress");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsMany("Json.Contact", "Contacts", b1 =>
                        {
                            b1.Property<int>("EmployeeId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Phone")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EmployeeId", "Id");

                            b1.ToTable("Employees");

                            b1.ToJson("Contacts");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");

                            b1.OwnsOne("Json.NotificationRules", "Rules", b2 =>
                                {
                                    b2.Property<int>("ContactEmployeeId")
                                        .HasColumnType("int");

                                    b2.Property<int>("ContactId")
                                        .HasColumnType("int");

                                    b2.Property<bool>("AllowCall")
                                        .HasColumnType("bit");

                                    b2.Property<bool>("AllowEmail")
                                        .HasColumnType("bit");

                                    b2.Property<bool>("AllowSms")
                                        .HasColumnType("bit");

                                    b2.Property<int>("MaximumMessagesPerDay")
                                        .HasColumnType("int");

                                    b2.HasKey("ContactEmployeeId", "ContactId");

                                    b2.ToTable("Employees");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactEmployeeId", "ContactId");
                                });

                            b1.Navigation("Rules")
                                .IsRequired();
                        });

                    b.OwnsOne("Json.Contact", "PrimaryContact", b1 =>
                        {
                            b1.Property<int>("EmployeeId")
                                .HasColumnType("int");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Phone")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employees");

                            b1.ToJson("PrimaryContact");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");

                            b1.OwnsOne("Json.NotificationRules", "Rules", b2 =>
                                {
                                    b2.Property<int>("ContactEmployeeId")
                                        .HasColumnType("int");

                                    b2.Property<bool>("AllowCall")
                                        .HasColumnType("bit");

                                    b2.Property<bool>("AllowEmail")
                                        .HasColumnType("bit");

                                    b2.Property<bool>("AllowSms")
                                        .HasColumnType("bit");

                                    b2.Property<int>("MaximumMessagesPerDay")
                                        .HasColumnType("int");

                                    b2.HasKey("ContactEmployeeId");

                                    b2.ToTable("Employees");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactEmployeeId");
                                });

                            b1.Navigation("Rules")
                                .IsRequired();
                        });

                    b.Navigation("BillingAddress")
                        .IsRequired();

                    b.Navigation("Contacts");

                    b.Navigation("PrimaryContact")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
