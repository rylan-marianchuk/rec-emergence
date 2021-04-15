using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// Containts frequently used system parameters for easy universal access
/// </summary>
public static class Settings
{
    // at some point when things r stable we should transfer all customizable parameter
    // references to this

    // Overall simulation settings

    public static int Categories { get; set; } = 10;

    public static int NumberAgents { get; set; } = 10;

    public static int DocumentsPerAgent { get; set; } = 20;

    // Recommender system settings
    public static int NumberSimilarAgents { get; set; } = 5;




}

