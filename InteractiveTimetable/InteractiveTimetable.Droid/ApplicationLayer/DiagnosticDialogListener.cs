
namespace InteractiveTimetable.Droid.ApplicationLayer
{
    public interface IDiagnosticDialogListener
    {
        void OnNewDiagnosticAdded(int diagnosticId);
        void OnDiagnosticEdited(int diagnosticId);
    }
}