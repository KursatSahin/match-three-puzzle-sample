namespace Containers
{
    public static class ContainerFacade
    {
        public static BoardSettingsContainer BoardSettings => BoardSettingsContainer.Instance;
        public static PrefabContainer PrefabContainer => PrefabContainer.Instance;
    }
}