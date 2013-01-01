Instructions
-------------------
Stick AlternateNames.txt and DoNotDraft.txt in your debug or release folder. The former fixes the difference in naming between Liquipedia and the FPL pages for various players. The latter ensures that players who cannot be drafted in the first week aren't picked by the program. Feel free to add entries to either file as required.

Using the program is fairly simple. Stick the URL of the Liquipedia page containg info for the current Proleague round in the URL textbox. The weeks textbox is there so you can find optimal teams for prior weeks.

When brute forcing, note that calculation time increases exponentially as the number of players considered increases. 40 players takes me just over two minutes to iterate through on my computer. In general, 20-30 will give you good results.


Notes
-------------------
I had to hardcode something to differentiate hero (on Liquid) from hero[join]. I'm not sure if anybody else suffers from similar naming issues. Keep an eye out.

I'm far more confident in the brute force solution than I am in the algorithm based solution. The algorithm based solution has worked so far though.

Trading is NOT YET SUPPORTED.
