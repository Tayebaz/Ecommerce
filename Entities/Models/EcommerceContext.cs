using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Entities.Models.Mapping;

namespace Entities.Models
{
    public partial class EcommerceContext : DbContext
    {
        static EcommerceContext()
        {
            Database.SetInitializer<EcommerceContext>(null);
        }

        public EcommerceContext()
            : base("Name=EcommerceContext")
        {
        }

        public DbSet<AddToCart> AddToCarts { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CompareList> CompareLists { get; set; }
        public DbSet<Help> Helps { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderedItem> OrderedItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SliderSetting> SliderSettings { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WishList> WishLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddToCartMap());
            modelBuilder.Configurations.Add(new BrandMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CompareListMap());
            modelBuilder.Configurations.Add(new HelpMap());
            modelBuilder.Configurations.Add(new ImageMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new OrderedItemMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductAttributeMap());
            modelBuilder.Configurations.Add(new ProductSizeMap());
            modelBuilder.Configurations.Add(new ReviewMap());
            modelBuilder.Configurations.Add(new SettingMap());
            modelBuilder.Configurations.Add(new SliderSettingMap());
            modelBuilder.Configurations.Add(new SubCategoryMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new WishListMap());
        }
    }
}
