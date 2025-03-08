using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace Trifoia.Module.InkPlayer.Migrations.EntityBuilders
{
    public class InkPlayerEntityBuilder : AuditableBaseEntityBuilder<InkPlayerEntityBuilder>
    {
        private const string _entityTableName = "TrifoiaInkPlayer";
        private readonly PrimaryKey<InkPlayerEntityBuilder> _primaryKey = new("PK_TrifoiaInkPlayer", x => x.InkPlayerId);
        private readonly ForeignKey<InkPlayerEntityBuilder> _moduleForeignKey = new("FK_TrifoiaInkPlayer_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public InkPlayerEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override InkPlayerEntityBuilder BuildTable(ColumnsBuilder table)
        {
            InkPlayerId = AddAutoIncrementColumn(table,"InkPlayerId");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            Name = AddMaxStringColumn(table,"Name");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> InkPlayerId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Name { get; set; }
    }
}
