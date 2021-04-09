AXImGui
=======

So this project is going to be house of ImGui in AcidarX. Currently it has almost nothing and it is in really, I mean *really* early stages of development. My goal is to have universal integration on ImGui inside AcidarX, or even try to have it not be dependent on AcidarX and be its own module with helper functionality for end-users to work with.

Example of what I would AxImGui to look like.

Currently ImGui does stuff like this
```c#
ImGui.Begin();
ImGui.Text("sample text");
ImGui.End();
```

Which I really dont like, having to type out `ImGui.Begin()` and `ImGui.End()` for client users could potentially be a pain. Also that'd mean we would have to ask users to install `ImGui.NET` package in their project, or include it ourselves somewhere, which I also dont like.

So my solution, which is really just in a early stages of thought process, would be to have something like this
```c#
AXImGui.Window((imGui) => {
    imgui.Text("sampleText")
});
```

You can see that in this sample, `.Window` would handle those `Begin/End` functions. and we would also be passing ImGui instance to the lambda for client applications to access. Obviously this idea might get scrapped as we start it's implementation.