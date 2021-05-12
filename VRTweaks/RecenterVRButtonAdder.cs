using UnityEngine.UI;
using HarmonyLib;
using System.Linq;
using TMPro;

[HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.Awake))]
public static class RecenterVRButtonAdder
{
    private static Button recenterVRButton;

    [HarmonyPostfix]
    public static void Postfix(IngameMenu __instance)
    {
        if (__instance != null && recenterVRButton == null)
        {
            //I think this is copying an existing button
            Button menuButton = __instance.quitToMainMenuButton.transform.parent.GetChild(0).gameObject.GetComponent<Button>();
            recenterVRButton = UnityEngine.Object.Instantiate<Button>(menuButton, __instance.quitToMainMenuButton.transform.parent);
            recenterVRButton.transform.SetSiblingIndex(1);//put the button in the second position in the menu
            recenterVRButton.name = "RecenterVR";
            recenterVRButton.onClick.RemoveAllListeners();//this seems to be removing listeners that would have been copied from the original button
                                                          //add new listener
            recenterVRButton.onClick.AddListener(delegate ()
            {
                VRUtil.Recenter();
            });
            //might be a better way to replace the text of the copied button
            var enumerable = recenterVRButton.GetComponents<TextMeshProUGUI>().Concat(recenterVRButton.GetComponentsInChildren<TextMeshProUGUI>());
            foreach (var text in enumerable)
            {
                text.text = "Recenter VR";
            }
        }
    }
}