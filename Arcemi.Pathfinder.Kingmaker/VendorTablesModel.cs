namespace Arcemi.Pathfinder.Kingmaker
{
    public class VendorTablesModel : RefModel
    {
        public VendorTablesModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<VendorTableModel> PersistentTables => A.List("m_PersistentTables", a => new VendorTableModel(a));
    }
}