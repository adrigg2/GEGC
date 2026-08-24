using GameBoyCEmulator.SaveState.Components;
using GameBoyCEmulator.SaveState.Components.APU;

namespace GameBoyCEmulator.SaveState;

public record SaveState(CPUState CPU, DMAState DMA, JOYPADState JOYPAD, MMUState MMU, PPUState PPU, TIMERState TIMER, APUState APU);
