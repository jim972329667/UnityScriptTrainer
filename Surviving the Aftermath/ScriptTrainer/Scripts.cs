using Aftermath.Cheats;
using Aftermath.Sim;
using Aftermath;
using System.Collections.Generic;
using UnityEngine;


namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static void SetAllPersonValue(PersonValueType valueType, float targetAmount)
        {
            List<Person> persons = Managers.Instance.City.Persons;
            if (persons.Count == 0)
            {
                Debug.LogWarning("No target persons to affect");
                return;
            }
            foreach (Person person in persons)
            {
                person.Values.GetPersonValue(valueType).Value = targetAmount;
            }
        }
        public static void ChangeHappiness()
        {
            SetAllPersonValue(PersonValueType.Happiness, 2f);
        }
    }
}
