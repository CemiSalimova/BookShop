using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.Data.EntityConfiguration
{
    using BookShop.Models;
    using Microsoft.EntityFrameworkCore;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(e => e.CategoryId);

            builder.Property(e => e.Name)
                .IsRequired(true)
                .IsUnicode(true)
                .HasMaxLength(50);
        }
    }
}
