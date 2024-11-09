using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // We use TMPro in order to modify the text on the tickets.

public class TicketMovement : MonoBehaviour
{
    // This script handles the movement of the tickets. It also creates ticket prefabs and handles the text on said tickets.

    #region Variables
    #region Reference-related Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;

    // We connect this script to the dispenser so we can keep track of its Y position.
    [SerializeField] private DispenserMovement dispenser;

    // Connects this script to stat data, however, it is currently connected to a temporary one that generates random numbers.
    [SerializeField] private DataExample statsPull;

    // The ticket prefab that we're going to spawn in multiple times to form a chain of connected tickets.
    [SerializeField] private GameObject ticketPrefab;
    #endregion

    #region Movement-based Variables
    private float posX; // X Position of group of tickets.
    private float startX; // The X position we want our group of tickets
    private float distanceLimit = 3f; // How far of a distance can the tickets travel before stopping?

    public bool ticketsMoving; // Is our ticket-moving method allowed to play out?

    private float speed = 30f; // How fast the tickets move to the right.
    private float speedDecrement; // How much we want to decrement the speed by at the end of the movement.
    #endregion

    #region Prefab-tracking Variables.
    private GameObject latestPrefab; // The last prefab we instantiated.
    private GameObject secondLastPrefab; // The second to last prefab we instantiated; for positioning.
    private int prefabCount = 0; // Keeps track of how many prefabs we have.
    private int prefabCountLimit = 6; // How many prefabs are allowed?
    #endregion

    #region Graphic-related Variables.
    private int ticketGraphic; // Which version of the ticket graphic should I display?
    private int graphicLimit; // When should I switch to the next ticket graphic?
    #endregion

    #region Stat-related Variables.
    [SerializeField] private int partyMemberIndex; // Index for stats (which slot/party member, I mean).
    
    private string statsHP = "";
    private string statsSP = "";
    private string statsDEF = "";
    private string statsATK = "";

    private string[] statsData;

    private int statIndex; // Stats data index.
    #endregion
    #endregion

    #region Body
    // Called before Start(), but only after being enabled.
    void OnEnable()
    {
        // Initialize starting X position.
        startX = dispenser.transform.position.x + 0.75f; // Position is relative to dispenser since it moves.

        // Tickets initially can not move.
        ticketsMoving = false;

        ticketGraphic = 3; // We initialize with 3 because first two tickets are already instantiated by default and already use ticketGraphic 4 (defense), so we work backwards from there.
        graphicLimit = 0;
    }

    void Start()
    {
        #region Position and Speed Initialization
        // Set X position to starting X position.
        posX = startX;

        // Take user speed and divide it by half (multiply it by two).
        speedDecrement = methods.DecimalsRounded(speed / 0.5f); // The reason this works is because we will be multiplying by fixed delta time later on, which, by default, is set to 0.02.

        // Initializes position of object using our variables.
        this.transform.position = new Vector3(methods.DecimalsRounded(posX), dispenser.transform.position.y, 0);
        #endregion

        // First ticket is initialized in the editor, but the first prefab is initialized here.
        #region Prefab Instantiation
        secondLastPrefab = null; // No object yet.

        // Prefab instantiation.
        latestPrefab = Instantiate(ticketPrefab, transform); // Instantiate a new instance of ticketPrefab with a transform and set it to our latest prefab so we can keep track of it.
        latestPrefab.GetComponent<TicketGraphic>().spriteIndex = 4; // Hard sets ticket graphic to defense ticket graphic.
        latestPrefab.transform.position = new Vector3(startX - 1.305f, dispenser.transform.position.y, 0f); // We position our first prefab a specific distance from the main ticket (do keep in mind that our main ticket is one pixel bigger than our ticket prefab).
        #endregion

        // Take stat data and convert it into string data for easier access.
        #region Stat Initialization
        statsHP = statsPull.statsData[partyMemberIndex][0].ToString();
        statsSP = statsPull.statsData[partyMemberIndex][1].ToString();
        statsATK = statsPull.statsData[partyMemberIndex][2].ToString();
        statsDEF = statsPull.statsData[partyMemberIndex][3].ToString();

        statsData = new string[8] { "HP", statsHP, "SP", statsSP, "ATK", statsATK, "DEF", statsDEF };

        statIndex = statsData.Length - 3;
        #endregion

        // Initializes only the first 2 tickets.
        #region Ticket Text Initialization
        if (statsData[statIndex] == "0" || statsData[statIndex + 1] == "0") // Checks if the stat string data for current ticket or the ticket to its right is "0". This is to acommodate for strings like, "HP", "AP", etc.
        {
            // Because stats should never be 0 for an active party member, stat strings that meet previously stated conditions should be blank instead.
            this.transform.Find("MainTicket/TicketText/Canvas/Text").GetComponent<TextMeshProUGUI>().text = ""; // DON'T FORGET, THIS SCRIPT MUST USE TMPro IN ORDER FOR THIS LINE TO WORK!!!
            latestPrefab.transform.Find("TicketText/Canvas/Text").GetComponent<TextMeshProUGUI>().text = ""; // DON'T FORGET, THIS SCRIPT MUST USE TMPro IN ORDER FOR THIS LINE TO WORK!!!
        }
        else
        {
            // Otherwise, render stored string data.
            this.transform.Find("MainTicket/TicketText/Canvas/Text").GetComponent<TextMeshProUGUI>().text = statsData[statIndex + 2]; // DON'T FORGET, THIS SCRIPT MUST USE TMPro IN ORDER FOR THIS LINE TO WORK!!!
            latestPrefab.transform.Find("TicketText/Canvas/Text").GetComponent<TextMeshProUGUI>().text = statsData[statIndex + 1]; // DON'T FORGET, THIS SCRIPT MUST USE TMPro IN ORDER FOR THIS LINE TO WORK!!!
        }
        #endregion
    }

    // Initially called before both Update() and LateUpdate(), and is called in timed steps.
    // Separate from frame-related update methods; used for physics-related updates for running consistently across different frame rates.
    void FixedUpdate()
    {
        // If tickets are allowed to move, then move our tickets with custom method.
        if (ticketsMoving)
        {
            MoveTickets();
        }

        // Sets position of object using our variables.
        this.transform.position = new Vector3(methods.DecimalsRounded(posX), dispenser.transform.position.y, 0);
    }
    #endregion

    #region Methods
    // This methods is responsible for moving our tickets and controlling their speed.
    void MoveTickets()
    {
        #region Call Spawn Method
        // Every time our latest prefab passes our X position limit, we instantiate a new instance of ticketPrefab and set it our new latest prefab.
        if (latestPrefab.transform.position.x > startX + 1f)
        {
            SpawnPrefabs(); // Spawn a new prefab with custom method.
        }
        #endregion

        #region Speed
        // If we've reached the distance limit, then decrement speed until we reach 0 for a smooth interpolated effect.
        if (this.transform.position.x >= startX + distanceLimit)
        {
            // Until our speed reaches 0 or lower, let's decrement our speed.
            if (speed > 0) // Interpolated movement.
            {
                // Because of math and how framerate works in relation, we need to split up the deacceleration of our speed (one half before and one half after our position update).
                // This rule only applies to speeds that change over time.
                // FixedUpdate() technically has a 1-frame delay, which I believe is why the order of these three lines is set up this way.
                speed -= (speedDecrement / 2) * Time.fixedDeltaTime; // Lower our speed.
                posX += speed * Time.fixedDeltaTime; // Move tickets.
                speed -= (speedDecrement / 2) * Time.fixedDeltaTime; // Lower our speed again with deacceleration rule.
            }

            // Now that we've done the calculation, we check if speed has accidentally gone below 0 before feeding value to X position.
            if (speed < 0) // Speed correction.
            {
                speed = 0;
                ticketsMoving = false;
            }
        }
        else // Default movement.
        {
            posX += speed * Time.fixedDeltaTime; // Move tickets with no change in speed.
        }
        #endregion
    }

    // This method is responsible for instantiating new ticket prefabs, tracking them, and managing them based on how many prefabs have already been instantiated (does not include first two tickets).
    void SpawnPrefabs()
    {
        #region Ticket Instantiation
        secondLastPrefab = latestPrefab; // Set new second-to-last prefab.

        latestPrefab = Instantiate(ticketPrefab, transform); // Instantiate a new instance of ticketPrefab with a transform and set it to our latest prefab so we can keep track of it.
        latestPrefab.GetComponent<TicketGraphic>().spriteIndex = ticketGraphic; // Set sprite index of latest ticket prefab to current ticket graphic.
        latestPrefab.transform.position = new Vector3(secondLastPrefab.transform.position.x - 1.3f, dispenser.transform.position.y, 0f); // We position our prefab a specific distance the second to last prefab's X position.
        #endregion

        #region Stat Rendering
        // Should the ticket use the data strings or not?
        if (statsData[statIndex] == "0" || statsData[statIndex + 1] == "0") // Checks if the stat string data for current ticket or the ticket to its right is "0". This is to acommodate for strings like, "HP", "AP", etc.
        {
            // Because stats should never be 0 for an active party member, stat strings that meet previously stated conditions should be blank instead.
            latestPrefab.transform.Find("TicketText/Canvas/Text").GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            // Otherwise, render stored string data.
            latestPrefab.transform.Find("TicketText/Canvas/Text").GetComponent<TextMeshProUGUI>().text = statsData[statIndex];
        }

        statIndex--; // Decrement stat index.

        // Quick way to avoid out of bounds error.
        if (statIndex < 0) // Should never reach negative.
        {
            statIndex = 0;
        }
        #endregion

        #region Graphic Updates
        // Update the ticket graphic data.
        if (ticketGraphic > 0) // Once we reach 0, stop decrementing (use 0 for the rest of the ticket prefabs), which also means we don't need the graphic limit anymore.
        {
            // The graphic limit is a delay that allows us to set two tickets with the same ticket graphic and then proceed to the next ticket graphic (ex. HP ticket graphic is set for HP ticket and HP stat ticket).
            if (graphicLimit == 1)
            {
                graphicLimit = 0; // Reset graphic limit.

                ticketGraphic--; // Next ticket graphic.
            }
            else
            {
                graphicLimit++;
            }
        }
        #endregion

        #region Ticket Text Handler
        // If we've already reached the previously set limit of ticket prefabs, then don't show text on any succeeding tickets.
        if (prefabCount < prefabCountLimit)
        {
            prefabCount++;
        }
        else
        {
            Destroy(latestPrefab.transform.GetChild(0).gameObject); // Destroys text on ticket prefab.
        }
        #endregion
    }
    #endregion
}
