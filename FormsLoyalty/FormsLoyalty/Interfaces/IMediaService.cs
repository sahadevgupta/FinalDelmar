using FormsLoyalty.Helpers.Media;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Interfaces
{
    public interface IMediaService
    {
        Task OpenGallery(int imageCount);

        /// <summary>
		/// Takes the picture.
		/// </summary>
		/// <param name="options">The storage options.</param>
		/// <returns>Task&lt;IMediaFile&gt;.</returns>
		Task<MediaFile> TakePhotoAsync(CameraMediaStorageOptions options);

    }

	#region Base Options

	/// <summary>
	///     Class MediaStorageOptions.
	/// </summary>
	public class MediaStorageOptions
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="MediaStorageOptions" /> class.
		/// </summary>
		protected MediaStorageOptions()
		{
		}

		/// <summary>
		///     Gets or sets the directory.
		/// </summary>
		/// <value>The directory.</value>
		public string Directory { get; set; }

		/// <summary>
		///     Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the maximum pixel dimension.
		/// </summary>
		/// <value>The maximum pixel dimension.</value>
		public int? MaxPixelDimension { get; set; }

		/// <summary>
		///     Gets or sets the percent quality.
		/// </summary>
		/// <value>The percent quality.</value>
		public int? PercentQuality { get; set; }
	}

	#endregion Base Options


	#region Camera Options

	/// <summary>
	///     Enum CameraDevice
	/// </summary>
	public enum CameraDevice
	{
		/// <summary>
		///     The rear
		/// </summary>
		Rear,

		/// <summary>
		///     The front
		/// </summary>
		Front
	}


	/// <summary>
	///     Class CameraMediaStorageOptions.
	/// </summary>
	public class CameraMediaStorageOptions : MediaStorageOptions
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="CameraMediaStorageOptions" /> class.
		/// </summary>
		public CameraMediaStorageOptions()
		{
			SaveMediaOnCapture = true;
		}

		/// <summary>
		///     Gets or sets the default camera.
		/// </summary>
		/// <value>The default camera.</value>
		public CameraDevice DefaultCamera { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether [save media on capture].
		/// </summary>
		/// <value><c>true</c> if [save media on capture]; otherwise, <c>false</c>.</value>
		public bool SaveMediaOnCapture { get; set; }
	}

#endregion Camera Options
}
