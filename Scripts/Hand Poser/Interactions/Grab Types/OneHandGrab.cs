namespace TedrickDev.XRPoser.Interactions
{
    public class OneHandGrab : GrabMode
    {
        private PoserHand activeHand;
        private bool isActive;
        
        public override void ApplyPose(GrabZone grabber)
        {
            // Initial grab
            if (!isActive) {
                isActive = true;
                activeHand = grabber.Hand;
                
                SetPose(grabber); 
                
            // Release
            } else if (isActive && grabber.Hand == activeHand) { 
                isActive = false;

                activeHand.ApplyDefaultPose();
                
                activeHand = grabber.Hand;
                transform.SetParent(null);
                
            // Opposite grab
            } else if (isActive && grabber.Hand != activeHand) { 
                activeHand.ApplyDefaultPose();
                activeHand = grabber.Hand;

                SetPose(grabber);
            }
        }
    }
}