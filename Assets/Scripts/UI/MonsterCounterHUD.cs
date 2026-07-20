using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCounterHUD : MonoBehaviour
{
    [Required][SerializeField] private TextMeshProUGUI _objectiveText;
    [Required][SerializeField] private Image _objectiveImage;
    [Required][SerializeField] private Sprite _objectiveCompletedSprite;
    private Player _player;
    private World _currentWorld;
    private Objective _currentObjective;


    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _currentWorld = _player.CurrentWorld;
        _currentObjective = _player.CurrentWorld.WorldData.Objective;
    }

    private void OnEnable()
    {
        _currentWorld.OnMonsterKilled += HandleMonsterKilled;
        _currentObjective.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        _currentWorld.OnMonsterKilled -= HandleMonsterKilled;
        _currentObjective.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private void Start()
    {
        // Ensure the objective text is set up
        if (_objectiveText == null)
        {
            Debug.LogError("Objective Text is not assigned in the HUD.");
            return;
        }

        Initiazlize();
    }

    // Update the HUD at the start
    private void Initiazlize()
    {
        if (_currentObjective != null)
        {
            UpdateObjectiveText();
        }
    }

    private void UpdateObjectiveText()
    {
        if (_currentObjective is KillObjective killObjective)
        {
            _objectiveText.text = $"{killObjective.RequiredKills - killObjective.CurrentKills} monsters left";
        }
    }

    // Update the HUD after a kill
    public void HandleMonsterKilled(Entity killedEntity)
    {
        if (_currentObjective is KillObjective)
        {
            if (!_currentObjective.IsCompleted())
            {
                UpdateObjectiveText();
            }
        }
    }

    // Update the HUD text when the objective is completed
    private void HandleObjectiveCompleted()
    {
        _objectiveText.text = "Completed!";
        _objectiveImage.sprite = _objectiveCompletedSprite;
        _objectiveImage.color = Color.green;
    }
}
