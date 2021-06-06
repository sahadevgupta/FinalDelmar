﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FormsLoyalty.Helpers.Media
{
	/// <summary>
	/// Class MediaFile. This class cannot be inherited.
	/// </summary>
	public sealed class MediaFile : IDisposable
	{
		#region Private Member Variables

		/// <summary>
		/// The _dispose
		/// </summary>
		private readonly Action<bool> _dispose;

		/// <summary>
		/// The _path
		/// </summary>
		private readonly string _path;

		/// <summary>
		/// The _stream getter
		/// </summary>
		private readonly Func<Stream> _streamGetter;

		/// <summary>
		/// The _is disposed
		/// </summary>
		private bool _isDisposed;

		#endregion Private Member Variables

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaFile" /> class.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="streamGetter">The stream getter.</param>
		/// <param name="dispose">The dispose.</param>
		public MediaFile(string path, Func<Stream> streamGetter, Action<bool> dispose = null)
		{
			_dispose = dispose;
			_streamGetter = streamGetter;
			_path = path;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="MediaFile" /> class.
		/// </summary>
		~MediaFile()
		{
			Dispose(false);
		}

		#endregion Constructors

		
		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <value>The path.</value>
		/// <exception cref="System.ObjectDisposedException">null</exception>
		public string Path
		{
			get
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException(null);
				}

				return _path;
			}
		}

		/// <summary>
		/// Gets the stream.
		/// </summary>
		/// <value>The source.</value>
		/// <exception cref="System.ObjectDisposedException">null</exception>
		public Stream Source
		{
			get
			{
				if (_isDisposed)
				{

					throw new ObjectDisposedException(null);
				}

				return _streamGetter();
			}
		}

		

		#region Public Methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
		/// unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			if (_dispose != null)
			{
				_dispose(disposing);
			}
		}

		#endregion Private Methods
	}
}
