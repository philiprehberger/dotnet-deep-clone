# Changelog

## [0.2.0] - 2026-04-05

- Add `CloneWith<T>(T obj, Action<T> configure)` for deep cloning with property overrides
- Add `DeepCloneWith<T>` extension method

## 0.1.2 (2026-03-31)

- Standardize README to 3-badge format with emoji Support section
- Update CI actions to v5 for Node.js 24 compatibility
- Add GitHub issue templates, dependabot config, and PR template

## 0.1.1 (2026-03-26)

- Add Sponsor badge and fix License link format in README

## 0.1.0 (2026-03-22)

- Initial release
- Deep cloning via compiled expression trees
- Support for primitives, strings, arrays, List<T>, Dictionary<K,V>, classes, and structs
- Circular reference detection using clone context
- Static `Clone<T>` method and `DeepClone()` extension method
