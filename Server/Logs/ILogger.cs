﻿/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2022 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: ILogger.cs                                                      *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System;

namespace Server.Logging;

public interface ILogger
{
    void Debug(string message, params object[] args);
    void Debug(Exception exception, string message, params object[] args);

    void Information(string message, params object[] args);
    void Information(Exception exception, string message, params object[] args);

    void Warning(string message, params object[] args);
    void Warning(Exception exception, string message, params object[] args);
    void Warning(Exception exception);

    void Error(string message, params object[] args);
    void Error(string message, bool sendToDiscord, params object[] args);
    void Error(Exception exception, string message, params object[] args);

    void Fatal(string message, params object[] args);
    void Fatal(Exception exception, string message, params object[] args);
}
