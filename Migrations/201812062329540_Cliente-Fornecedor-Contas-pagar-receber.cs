namespace BarberSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClienteFornecedorContaspagarreceber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CONTAS_PAGAR", "cliente_codigo", c => c.Int());
            AddColumn("dbo.CONTAS_RECEBER", "cliente_codigo", c => c.Int());
            CreateIndex("dbo.CONTAS_PAGAR", "cliente_codigo");
            CreateIndex("dbo.CONTAS_RECEBER", "cliente_codigo");
            AddForeignKey("dbo.CONTAS_PAGAR", "cliente_codigo", "dbo.CLIENTES", "codigo");
            AddForeignKey("dbo.CONTAS_RECEBER", "cliente_codigo", "dbo.CLIENTES", "codigo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CONTAS_RECEBER", "cliente_codigo", "dbo.CLIENTES");
            DropForeignKey("dbo.CONTAS_PAGAR", "cliente_codigo", "dbo.CLIENTES");
            DropIndex("dbo.CONTAS_RECEBER", new[] { "cliente_codigo" });
            DropIndex("dbo.CONTAS_PAGAR", new[] { "cliente_codigo" });
            DropColumn("dbo.CONTAS_RECEBER", "cliente_codigo");
            DropColumn("dbo.CONTAS_PAGAR", "cliente_codigo");
        }
    }
}
