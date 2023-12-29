namespace HackingOps.Common.Settings
{
    public interface ISetting
    {
        void ResetValue();
        void ApplyPrevious();
        void ApplyBlueprint();
        void SetBlueprintValue<T>(T blueprintValue);
        void DiscardValue();
        void Apply();
        void ApplyAsBlueprint();
    }
}