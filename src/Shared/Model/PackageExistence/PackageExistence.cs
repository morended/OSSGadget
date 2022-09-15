// Copyright (c) Microsoft Corporation. Licensed under the MIT License.

namespace Microsoft.CST.OpenSource.Model.PackageExistence;

using Contracts;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represents a package that currently exists.
/// </summary>
public record PackageExists : IPackageExistence
{
    public bool Exists => true;
    public bool HasEverExisted => true;
}

/// <summary>
/// Represents a package that never existed.
/// </summary>
public record PackageNotFound : IPackageExistence
{
    public bool Exists => false;
    public virtual bool HasEverExisted => false;
}

/// <summary>
/// Represents a package that was removed, and why.
/// </summary>
/// <param name="RemovalReasons">The reasons (if any) for why a package was removed.</param>
public record PackageRemoved(IReadOnlySet<PackageRemovalReason> RemovalReasons) : PackageNotFound
{
    public override bool HasEverExisted => true;

    protected override bool PrintMembers(StringBuilder stringBuilder)
    {
        if (base.PrintMembers(stringBuilder))
        {
            stringBuilder.Append(", ");
        }

        string reasons = string.Join(',', this.RemovalReasons.Select(r => r.ToString()));

        stringBuilder.Append($"RemovalReasons = [{reasons}]");
        return true;
    }
}