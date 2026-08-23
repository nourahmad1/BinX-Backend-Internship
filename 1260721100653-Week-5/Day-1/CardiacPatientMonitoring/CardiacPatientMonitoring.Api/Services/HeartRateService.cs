namespace CardiacPatientMonitoring.Api.Services;

public class HeartRateService
{
    // Checks whether a heart rate is within the safe range
    // used by our application.
    public bool IsHeartRateValid(int heartRate)
    {
        return heartRate >= 40 && heartRate <= 200;
    }
}