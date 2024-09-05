using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message.Domain.MessageAggregate;

namespace Message.Infrastructure.EntityConfigurations;

internal class CommonEntityTypeConfiguration : IEntityTypeConfiguration<CommonMessage>
{
    public void Configure(EntityTypeBuilder<CommonMessage> builder)
    {
        // Table configuration
        builder.ToTable("Common", "dbo");

        // Primary key
        builder.HasKey(c => new { c.msg_target, c.m_type });

        // Columns configuration
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property<string>("_msg_status")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("msg_status")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property<string>("_msg_source")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("msg_source")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.msg_target)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("msg_target")
            .IsRequired();

        builder.Property<string>("_prty")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("prty")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.m_type)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("m_type")
            .IsRequired();

        builder.Property<string>("_ref_source")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ref_source")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property<string>("_ref_request_id")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ref_request_id")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property<int?>("_ref_seq_no")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ref_seq_no");

        builder.Property<DateTime>("_dt_created")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("dt_created")
            .IsRequired();

        // Unique Index configuration
        builder.HasIndex(c => new { c.msg_target, c.m_type })
            .IsUnique()
            .HasDatabaseName("UK_Common");

        //// Foreign Key configuration
        //builder.HasOne(c => c.RSIs)
        //    .WithMany() // assuming Rsi does not have a navigation property to Common
        //    .HasForeignKey(c => new { c.m_type, c.msg_target })
        //    .HasConstraintName("FK_Common_messageTypeLookup_id")
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}