namespace BarberSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class produtos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PRODUTOS", "quantidade", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PRODUTOS", "quantidade");
        }
    }
}
