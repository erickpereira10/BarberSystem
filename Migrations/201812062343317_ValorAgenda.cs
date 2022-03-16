namespace BarberSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValorAgenda : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AGENDA", "valor", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AGENDA", "valor");
        }
    }
}
