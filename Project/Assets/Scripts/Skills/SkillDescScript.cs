using UnityEngine;
[CreateAssetMenu(fileName = "Skill", menuName = "Custom/Skills")]
public class SkillDescScript : ScriptableObject
{
    public Skill Data;

    public string skillName = "";
    public bool isPassive = false;
    public Texture skillImage;
    [TextArea]
    public string langDesc = "";

    string[] statsGrown;

    bool isFlatStat = false;
    float[] fristStatMultiplier;
    float[] secondStatMultiplier;
    float[] terthStatMultiplier;

    public bool isCustom = false;

    public void Apply(int level)
    {
        if (isPassive)
        {
            int index = 0;
            float multiplier = 1;
            foreach (string statchang in statsGrown)
            {
                index++;
                switch (index)
                {
                    case 1:
                        multiplier = fristStatMultiplier[(level - 1)];
                        break;
                    case 2:
                        multiplier = secondStatMultiplier[(level - 1)];
                        break;
                    case 3:
                        multiplier = terthStatMultiplier[(level - 1)];
                        break;
                }

                // (int)multiplier);

            }
        }
    }

    public string processDesc(int level = 1)
    {

        // langDesc

        string text = langDesc;
        int index = 0;
        float multiplier = 1;


        return text;
    }

}