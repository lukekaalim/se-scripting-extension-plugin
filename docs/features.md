# Features

## Fallback

When at least one script module is detected in a mod,
the regular code inside `Data/Scripts` is ignored.

That way, when a user downloads a mod that utitlizes
script modules, but doesnt have the Scripting Extension Plugin
installed, you can gracefully fallback to previous behaviour.

## Known Issues

- [ ] Mod script performance metrics are not working.
- [ ] Mod syntax blacklisting/type whitelisting is not working. (Security sandbox broken)
- [ ] Doesnt work for Mod.IO
- [ ] Doesnt respect any flag that disabled mods or scripting
- [ ] Everything is delciously synchronous